﻿using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DTO;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using Microsoft.AspNetCore.Http;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ConsensusBLL : IConsensusBLL
    {
        private readonly IConsensusDAL consensusDAL;
        private readonly IConsensusInstanceDAL consensusInstanceDAL;
        private readonly IOutsideUserDAL outsideUserDAL;
        private readonly IPersonnelDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IEmailSender emailSender;
        private readonly IMapper Mapper; 
        private readonly ICodeDAL codeDAL;

        public ConsensusBLL(IConsensusDAL consensusDAL, IConsensusInstanceDAL consensusInstanceDAL, IOutsideUserDAL outsideUserDAL, IPersonnelDAL userDAL, IOrganizationDAL organizationDAL, IFormDAL formDAL, ICodeDAL codeDAL, IEmailSender emailSender, IMapper mapper)
        {
            this.consensusDAL = consensusDAL;
            this.outsideUserDAL = outsideUserDAL;
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
            this.formDAL = formDAL;
            this.codeDAL = codeDAL;
            this.consensusInstanceDAL = consensusInstanceDAL;
            this.emailSender = emailSender;
            Mapper = mapper;
        }

        public ResourceCreatedDTO StartNewIteration(string consensusId, string formId, int creatorId)
        {
            Consensus consensus;
            if (string.IsNullOrWhiteSpace(consensusId))
            {
                consensus = CreateConsensusAndStartIteration(formId, creatorId);
            }
            else
            {
                consensus = TryStartIteration(consensusId);
            }

            return new ResourceCreatedDTO()
            {
                Id = consensus?.Id,
                LastUpdate = consensus.LastUpdate.Value.ToString("o")
            };
        }

        public ResourceCreatedDTO TerminateCurrentIteration(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().State = IterationState.Terminated;
            consensusDAL.Insert(consensus);

            SendMailToConsensusCreator(consensus, IterationState.Terminated);

            return new ResourceCreatedDTO()
            {
                Id = consensus?.Id,
                LastUpdate = consensus.LastUpdate.Value.ToString("o")
            };
        }

        public void RemindUser(RemindUserDataIn remindUserDataIn)
        {
            ConsensusUserMailDTO consensusUserMailDTO = new ConsensusUserMailDTO
            {
                ConsensusId = remindUserDataIn.ConsensusId,
                IsOutsideUser = remindUserDataIn.IsOutsideUser,
                IterationId = remindUserDataIn.IterationId,
                User = GetConsensusUserData(remindUserDataIn.UserId, remindUserDataIn.IsOutsideUser)
            };

            string mailContent = EmailHelpers.GetConsensusParticipantReminderEmailContent(consensusUserMailDTO);
            Task.Run(() => emailSender.SendAsync(new EmailDTO(consensusUserMailDTO.User.Email, mailContent, $"{EmailSenderNames.SoftwareName} Consensus Finding Reminder")));
        }

        public void StartConsensusFindingProcess(ConsensusFindingProcessDataIn dataIn)
        {
            Consensus consensus = consensusDAL.GetById(dataIn.ConsensusId);
            ShouldConsensusFindingProcessStop(consensus);
            ConsensusIteration iteration = consensus.Iterations.Last();
            iteration.State = IterationState.InProgress;
            consensusDAL.Insert(consensus);

            Form form = formDAL.GetForm(consensus.FormRef);

            SendMailToConsensusCreator(consensus, IterationState.InProgress);
            SendMailsToUsers(dataIn, form, true, iteration.Id);
            SendMailsToUsers(dataIn, form, false, iteration.Id);
        }

        public ConsensusDataOut ProceedIteration(ProceedConsensusDataIn proceedConsensusDataIn)
        {
            Consensus consensus = consensusDAL.GetById(proceedConsensusDataIn.ConsensusId);
            ConsensusIteration consensusIteration = consensus.GetIterationById(proceedConsensusDataIn.IterationId);

            consensusIteration.State = IterationState.Design;
            consensusIteration.QuestionOccurences = proceedConsensusDataIn.QuestionOccurences.Select(x =>
            new QuestionOccurenceConfig()
            {
                Level = x.Level,
                Type = x.Type
            })
            .ToList();

            consensusDAL.Insert(consensus);

            return Mapper.Map<ConsensusDataOut>(consensus);
        }

        public ConsensusDataOut GetById(string id)
        {
            return Mapper.Map<ConsensusDataOut>(consensusDAL.GetById(id));
        }

        public void AddQuestion(ConsensusQuestionDataIn consensusQuestionDataIn)
        {
            ConsensusQuestion question = Mapper.Map<ConsensusQuestion>(consensusQuestionDataIn);
            Consensus consensus = consensusDAL.GetByFormId(consensusQuestionDataIn.FormId);
            ConsensusIteration iteration = consensus.Iterations.Last();
            iteration.Questions.Add(question);
            RepeatQuestion(consensusQuestionDataIn, iteration);
            consensusDAL.Insert(consensus);
        }

        public ResourceCreatedDTO SubmitConsensusInstance(ConsensusInstanceDataIn consensusInstanceDataIn)
        {
            ConsensusInstance consensusInstance = Mapper.Map<ConsensusInstance>(consensusInstanceDataIn);
            if (!consensusDAL.IsLastIterationFinished(consensusInstanceDataIn.ConsensusRef))
            {
                consensusInstanceDAL.InsertOrUpdate(consensusInstance);
                NotifyConsensusCreatorIfIterationIsCompleted(consensusInstance);
            }
        
            return new ResourceCreatedDTO() { Id = consensusInstance.Id };
        }

        public TrackerDataOut GetTrackerData(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            List<ConsensusInstance> consensusInstances = consensusInstanceDAL.GetAllByConsensusId(consensusId);
            List<Personnel> users = userDAL.GetAllByIds(consensus.GetAllUserIds(false));
            List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers = outsideUserDAL.GetAllByIds(consensus.GetAllUserIds(true));

            return new TrackerDataOut()
            {
                ConsensusId = consensusId,
                ActiveIterationId = consensus.Iterations.Last().Id,
                Iterations = GetIterationTrackerData(consensus, consensusInstances, users, outsideUsers)
            };
        }

        public List<UserDataOut> SaveUsers(List<int> usersIds, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().UserIds.AddRange(usersIds);
            consensus.Iterations.Last().UserIds = consensus.Iterations.Last().UserIds.Distinct().ToList();
            consensusDAL.Insert(consensus);

            List<UserDataOut> users = GetInsideUsersByConsensus(consensus);

            return users;
        }

        public ConsensusUsersDataOut GetConsensusUsers(string consensusId, int activeOrganization)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);

            return new ConsensusUsersDataOut
            {
                ConsensusOrganizationUserInfoData = GetOrganizationUserInfo(activeOrganization),
                OrganizationUsersCount = Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationDAL.GetOrganizationUsersCount(null, null)),
                Users = GetInsideUsersByConsensus(consensus),
                OutsideUsers = GetOutsideUsers(consensusId)
            };
        }

        public bool CanSubmitConsensusInstance(ConsensusInstanceDataIn consensusInstance)
        {
            bool canSubmit = true;
            ConsensusIteration currentIteration = consensusDAL.GetLastIteration(consensusInstance.ConsensusRef);
            if (!consensusInstance.IsOutsideUser)
            {
                if (!userDAL.UserHasPermission(consensusInstance.UserRef, ModuleNames.Designer, PermissionNames.FindConsensus)) throw new UserAdministrationException(StatusCodes.Status409Conflict, $"User does not have permission {PermissionNames.FindConsensus} in {ModuleNames.Designer} module");
                if (!currentIteration.UserIds.Contains(consensusInstance.UserRef)) throw new UserAdministrationException(StatusCodes.Status409Conflict, "User does not have belong to the current iteration");
            }

            return canSubmit;
        }

        private Consensus CreateConsensusAndStartIteration(string formId, int creatorId)
        {
            Consensus consensus = new Consensus
            {
                FormRef = formId,
                CreatorId = creatorId,
                State = ConsensusFindingState.OnGoing,
                Iterations = new List<ConsensusIteration>()
                {
                    new ConsensusIteration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserIds = new List<int>(),
                        OutsideUserIds = new List<int>(),
                        Questions = new List<ConsensusQuestion>(),
                        State = IterationState.NotStarted,
                        EntryDatetime = DateTime.Now
                    }
                }
            };
            consensusDAL.Insert(consensus);

            return consensus;
        }

        private UserViewDataOut GetConsensusUserData(int userId, bool isOutsideUser)
        {
            if (isOutsideUser)
            {
                OutsideUser outsideUser = outsideUserDAL.GetById(userId);
                return new UserViewDataOut
                {
                    Id = userId,
                    Email = outsideUser.Email,
                    FirstName = outsideUser.FirstName,
                    LastName = outsideUser.LastName
                };
            } 
            else
            {
                Personnel personnel = userDAL.GetById(userId);
                return new UserViewDataOut
                {
                    Id = userId,
                    Email = personnel.Email,
                    FirstName = personnel.FirstName,
                    LastName = personnel.LastName
                };
            }
        }

        private void SendMailsToUsers(ConsensusFindingProcessDataIn dataIn, Form form, bool isOutsideUsers, string iterationId)
        {
            Dictionary<int, string> users = isOutsideUsers ?
                    outsideUserDAL.GetAllByIds(dataIn.OutsideUsersIds).ToDictionary(x => x.OutsideUserId, x => x.Email)
                    : userDAL.GetAllByIds(dataIn.UsersIds).ToDictionary(x => x.PersonnelId, x => x.Email);

            string fullFormNameWithVersion = $"{form.Title} {form.Version.GetFullVersionString()}";

            foreach (KeyValuePair<int, string> user in users)
            {
                ConsensusUserMailDTO consensusUserMailDTO = new ConsensusUserMailDTO
                {
                    ConsensusId = dataIn.ConsensusId,
                    IsOutsideUser = isOutsideUsers,
                    IterationId = iterationId,
                    User = GetConsensusUserData(user.Key, isOutsideUsers),
                    CustomMessage = dataIn.EmailMessage,
                    FullFormNameWithVersion = fullFormNameWithVersion
                };
                string mailContent = EmailHelpers.GetConsensusParticipantEmailContent(consensusUserMailDTO);
                Task.Run(() => emailSender.SendAsync(new EmailDTO(user.Value, mailContent, $"{EmailSenderNames.SoftwareName} Consensus Finding Process")));
            }
        }

        private void SendMailToConsensusCreator(Consensus consensus, IterationState iterationState)
        {
            Personnel creator = userDAL.GetById(consensus.CreatorId);
            if(creator != null)
            {
                Form form = formDAL.GetForm(consensus.FormRef);
                ConsensusIteration iteration = consensus.Iterations.Last();
                List<Personnel> users = userDAL.GetAllByIds(iteration.UserIds);
                List<OutsideUser> outsideUsers = outsideUserDAL.GetAllByIds(iteration.OutsideUserIds);

                string mailContent = EmailHelpers.GetConsensusCreatorEmailContent(consensus, form, iterationState, creator, users, outsideUsers);
                Task.Run(() => emailSender.SendAsync(new EmailDTO(creator.Email, mailContent, $"{EmailSenderNames.SoftwareName} Consensus Finding Process")));
            }
        }

        private Consensus TryStartIteration(string consensusId)
        {
            Consensus consensus;
            if (consensusDAL.IsLastIterationFinished(consensusId))
            {
                string id = Guid.NewGuid().ToString();
                consensus = consensusDAL.GetById(consensusId);
                consensus.Iterations.Add(new ConsensusIteration()
                {
                    Id = id,
                    Questions = new List<ConsensusQuestion>(),
                    UserIds = consensus.Iterations.Last().UserIds,
                    OutsideUserIds = consensus.Iterations.Last().OutsideUserIds,
                    State = IterationState.NotStarted,
                    EntryDatetime = DateTime.Now
                });

                consensus.SetIterationsState(id);
                consensusDAL.Insert(consensus);
            }
            else
            {
                throw new IterationNotFinishedException();
            }

            return consensus;
        }

        private List<ConsensusQuestion> GetQuestions(ConsensusQuestionDataIn consensusQuestionDataIn, List<string> ids)
        {
            List<ConsensusQuestion> result = new List<ConsensusQuestion>();
            ids.ForEach(x =>
                                         {
                                             result.Add(new ConsensusQuestion()
                                             {
                                                 Comment = consensusQuestionDataIn.Comment,
                                                 ItemRef = x,
                                                 Options = consensusQuestionDataIn.Options,
                                                 Question = consensusQuestionDataIn.Question,
                                                 Value = consensusQuestionDataIn.Value
                                             });
                                         });

            return result;
        }

        private void RepeatQuestion(ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            QuestionOccurenceConfig questionOccurenceConfig = iteration.QuestionOccurences?.Find(x => x.Level == consensusQuestionDataIn.Level.ToString());
            if (questionOccurenceConfig != null && questionOccurenceConfig.Type == QuestionOccurenceType.Same)
            {
                Form form = this.formDAL.GetForm(consensusQuestionDataIn.FormId) ?? throw new EntryPointNotFoundException();
                switch (consensusQuestionDataIn.Level)
                {
                    case FormItemLevel.Chapter:
                        RepeatChapterQuestion(form, consensusQuestionDataIn, iteration);
                        break;

                    case FormItemLevel.Page:
                        RepeatPageQuestion(form, consensusQuestionDataIn, iteration);
                        break;

                    case FormItemLevel.FieldSet:
                        RepeatFieldSetQuestion(form, consensusQuestionDataIn, iteration);
                        break;

                    case FormItemLevel.Field:
                        RepeatFieldQuestion(form, consensusQuestionDataIn, iteration);
                        break;

                    case FormItemLevel.FieldValue:
                        RepeatFieldValuesQuestion(form, consensusQuestionDataIn, iteration);
                        break;
                }
            }
        }

        private void RepeatChapterQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var chapters = form.Chapters
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, chapters));
        }

        private void RepeatPageQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var pages = form.Chapters
                .SelectMany(x => x.Pages)
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, pages));
        }

        private void RepeatFieldSetQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var fieldSets = form.GetAllFieldSets()
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, fieldSets));
        }

        private void RepeatFieldQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var fields = form.GetAllFields()
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, fields));
        }

        private void RepeatFieldValuesQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var fieldValues = form.GetAllFields()
                .OfType<FieldSelectable>()
                .SelectMany(x => x.Values)
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, fieldValues));
        }

        private void ShouldConsensusFindingProcessStop(Consensus consensus)
        {
            ConsensusIteration currentConsensusIteration = consensus.Iterations.Last();
            List<QuestionOccurenceConfig> questionOccurenceConfigList = currentConsensusIteration.QuestionOccurences;
            if(questionOccurenceConfigList != null)
            {
                foreach (QuestionOccurenceConfig questionOccurenceConfig in questionOccurenceConfigList)
                {
                    QuestionOccurenceType occurenceType = questionOccurenceConfig.Type;
                    if (occurenceType == QuestionOccurenceType.Different || occurenceType == QuestionOccurenceType.Same)
                    {
                        Form form = this.formDAL.GetForm(consensus.FormRef);

                        string occurenceLevel = questionOccurenceConfig.Level;
                        switch ((FormItemLevel)Enum.Parse(typeof(FormItemLevel), occurenceLevel))
                        {
                            case FormItemLevel.Form:
                                List<string> formIds = new List<string> { form.Id };
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, formIds)) throw new ConsensusCannotStartException(FormItemLevel.Form);
                                break;
                            case FormItemLevel.Chapter:
                                List<string> chapterIds = form.Chapters.Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, chapterIds)) throw new ConsensusCannotStartException(FormItemLevel.Chapter);
                                break;

                            case FormItemLevel.Page:
                                List<string> pageIds = form.GetAllPages().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, pageIds)) throw new ConsensusCannotStartException(FormItemLevel.Page);
                                break;

                            case FormItemLevel.FieldSet:
                                List<string> fieldSetIds = form.GetAllFieldSets().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, fieldSetIds)) throw new ConsensusCannotStartException(FormItemLevel.FieldSet);
                                break;

                            case FormItemLevel.Field:
                                List<string> fieldIds = form.GetAllFields().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, fieldIds)) throw new ConsensusCannotStartException(FormItemLevel.Field);
                                break;

                            case FormItemLevel.FieldValue:
                                List<string> fieldValueIds = form.GetAllFieldValues().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, fieldValueIds)) throw new ConsensusCannotStartException(FormItemLevel.FieldValue);
                                break;
                        }
                    }
                }
            }
        }

        private bool AllQuestionsCoveredFor(ConsensusIteration consensusIteration, List<string> formElementIds)
        {
            int numOfMissingQuestions = formElementIds.Count(itemRefId => !consensusIteration.Questions.Exists(q => q.ItemRef.Equals(itemRefId)));
            return numOfMissingQuestions == 0;
        }

        private void NotifyConsensusCreatorIfIterationIsCompleted(ConsensusInstance consensusInstance)
        {
            List<ConsensusInstance> consensusInstances = consensusInstanceDAL.GetAllByConsensusAndIteration(consensusInstance.ConsensusRef, consensusInstance.IterationId);
            Consensus consensus = consensusDAL.GetById(consensusInstance.ConsensusRef);
            ConsensusIteration consensusIteration = consensus.GetIterationById(consensusInstance.IterationId);
            bool allQuestionnairesInitialized = consensusInstances.Count == consensusIteration.OutsideUserIds.Count + consensusIteration.UserIds.Count;
            bool allQuestionnairesCompleted = consensusInstances.TrueForAll(c => c.GetPercentDone() == 100.00);
            if (allQuestionnairesInitialized && allQuestionnairesCompleted)
            {
                SendMailToConsensusCreator(consensus, IterationState.Finished);
                consensusIteration.State = IterationState.Finished;
                consensusDAL.Insert(consensus);
            }
        }

        private List<IterationTrackerDataOut> GetIterationTrackerData(Consensus consensus, List<ConsensusInstance> consensusInstances, List<Personnel> insideUsers, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers)
        {
            List<IterationTrackerDataOut> result = new List<IterationTrackerDataOut>();
            foreach (ConsensusIteration iteration in consensus.Iterations.Where(x => x.State != IterationState.NotStarted && x.State != IterationState.Design).ToList())
            {
                IterationTrackerDataOut iterationTracker = GetSingleIterationTracker(consensus.Id, iteration, insideUsers, outsideUsers, consensusInstances);
                result.Add(iterationTracker);
            }

            return result;
        }

        private IterationTrackerDataOut GetSingleIterationTracker(string consensusId, ConsensusIteration iteration, List<Personnel> insideUsers, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers, List<ConsensusInstance> consensusInstances)
        {
            IterationTrackerDataOut iterationTracker = new IterationTrackerDataOut(iteration.Id)
            {
                State = iteration.State
            };

            GetSingleIterationInstancesTrackerForInsideUsers(iterationTracker, consensusId, iteration, insideUsers, consensusInstances);
            GetSingleIterationInstancesTrackerForOutsideUsers(iterationTracker, consensusId, iteration, outsideUsers, consensusInstances);

            return iterationTracker;
        }

        private void GetSingleIterationInstancesTrackerForInsideUsers(IterationTrackerDataOut iterationTracker, string consensusId, ConsensusIteration iteration, List<Personnel> insideUsers, List<ConsensusInstance> consensusInstances)
        {
            foreach (int userId in iteration.UserIds)
            {
                Personnel user = insideUsers.Find(x => x.PersonnelId == userId);
                ConsensusInstance consensusInstance = consensusInstances.Find(x => x.UserId == userId && x.ConsensusRef == consensusId.ToString() && x.IterationId == iteration.Id);
                iterationTracker.Instances.Add(GetSingleInstanceTracker(user.PersonnelId, $"{user.GetFirstAndLastName()}", false, consensusInstance));
            }
        }

        private void GetSingleIterationInstancesTrackerForOutsideUsers(IterationTrackerDataOut iterationTracker, string consensusId, ConsensusIteration iteration, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers, List<ConsensusInstance> consensusInstances)
        {
            foreach (int userId in iteration.OutsideUserIds)
            {
                Domain.Sql.Entities.OutsideUser.OutsideUser outsideUser = outsideUsers.Find(x => x.OutsideUserId == userId);
                ConsensusInstance consensusInstance = consensusInstances.Find(x => x.UserId == userId && x.ConsensusRef == consensusId.ToString() && x.IterationId == iteration.Id);
                iterationTracker.Instances.Add(GetSingleInstanceTracker(outsideUser.OutsideUserId, $"{outsideUser.FirstName} {outsideUser.LastName}", true, consensusInstance));
            }
        }

        private ConsensusInstanceTrackerDataOut GetSingleInstanceTracker(int userId, string userName, bool isOutsideUser, ConsensusInstance consensusInstance)
        {
            double percentDone = consensusInstance != null ? consensusInstance.GetPercentDone() : 0;

            return new ConsensusInstanceTrackerDataOut(userId, userName, isOutsideUser, consensusInstance?.EntryDatetime, consensusInstance?.LastUpdate, percentDone);
        }

        private List<ConsensusUserDataOut> GetOutsideUsers(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            List<ConsensusUserDataOut> outsideUsers = Mapper.Map<List<ConsensusUserDataOut>>(outsideUserDAL.GetAllByIds(consensus.Iterations.Last().OutsideUserIds));
            return outsideUsers;
        }

        private ConsensusOrganizationUserInfoDataOut GetOrganizationUserInfo(int activeOrganization)
        {
            int? countryCD = organizationDAL.GetById(activeOrganization)?.OrganizationAddress?.CountryCD;
            ConsensusOrganizationUserInfoDataOut data = new ConsensusOrganizationUserInfoDataOut()
            {
                UsersCount = (int)userDAL.GetAllCount(),
                OrganizationsCount = (int)organizationDAL.GetAllCount(),
                OrganizationsCountByState = (int)organizationDAL.GetAllEntriesCountByCountry(countryCD.GetValueOrDefault())
            };
            return data;
        }

        private List<UserDataOut> GetInsideUsersByConsensus(Consensus consensus)
        {
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(consensus.Iterations.Last().UserIds));
            List<int> organizationsIds = GetOrganizationsIds(users);
            List<OrganizationDataOut> organizations = Mapper.Map<List<OrganizationDataOut>>(organizationDAL.GetByIds(organizationsIds));
            SetOrganizationsForUsers(users, organizations);

            return users;
        }

        private List<int> GetOrganizationsIds(List<UserDataOut> users)
        {
            List<int> result = new List<int>();
            foreach (var user in users)
            {
                result.AddRange(user.GetOrganizationRefs());
            }

            return result.Distinct().ToList();
        }

        private void SetOrganizationsForUsers(List<UserDataOut> users, List<OrganizationDataOut> organizations)
        {
            int? archivedUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            foreach (var user in users)
            {
                foreach (var organization in user.GetNonArchivedOrganizations(archivedUserStateCD))
                {
                    organization.Organization = organizations.Find(x => x.Id == organization.OrganizationId);
                }
            }
        }
    }
}
