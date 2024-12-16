using AutoMapper;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class ThesaurusEntryBLL : IThesaurusEntryBLL
    {
        private const string MergeCodes = "Codes";
        private const string MergeDefinitions = "Definition";
        private const string MergeAbbreviations = "Abbreviations";
        private const string MergeSynonyms = "Synonyms";
        private const string MergeTranslations = "Translations";

        public void TakeBoth(int currentId)
        {
            int? productionStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusState, CodeAttributeNames.Production);
            thesaurusDAL.UpdateState(currentId, productionStateCD);
        }

        public void MergeThesauruses(ThesaurusMergeDataIn thesaurusMergeDataIn, UserData userData)
        {
            ThesaurusEntry sourceThesaurus = thesaurusDAL.GetById(thesaurusMergeDataIn.CurrentId);
            ThesaurusEntry targetThesaurus = thesaurusDAL.GetById(thesaurusMergeDataIn.TargetId) ?? throw new ArgumentNullException($"Target thesaurus with given id (id=${thesaurusMergeDataIn.TargetId}) does not exist");

            MergeValues(sourceThesaurus, targetThesaurus, thesaurusMergeDataIn.ValuesForMerge, userData);

            thesaurusMergeDAL.InsertOrUpdate(Mapper.Map<ThesaurusMerge>(thesaurusMergeDataIn));
        }

        public int MergeThesaurusOccurences(UserCookieData userCookieData)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                UserData userData = Mapper.Map<UserData>(userCookieData);
                int numOfUpdatedEntries = 0;
                int? pendingStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusMergeState, CodeAttributeNames.Pending);

                foreach (ThesaurusMerge thesaurusMerge in thesaurusMergeDAL.GetAllByState(pendingStateCD))
                {
                    numOfUpdatedEntries += Replace(thesaurusMerge, userData);
                    TryDelete(thesaurusMerge.OldThesaurus, applyUsingThesaurusInCodeCheck: false, organizationTimeZone: userCookieData.OrganizationTimeZone);
                }
                DateTime endTime = DateTime.Now;
                SendEmail(userCookieData, startTime, endTime);

                return numOfUpdatedEntries;
            }
            catch (Exception ex)
            {
                DateTime endTime = DateTime.Now;
                SendEmail(userCookieData, startTime, endTime, noErrors: false);
                throw;
            }
        }

        #region Merge Thesaurus Values
        private void MergeValues(ThesaurusEntry sourceThesaurus, ThesaurusEntry targetThesaurus, List<string> valuesForMerge, UserData userData)
        {
            if (valuesForMerge != null)
            {
                UpdateMergeListIfThereAreDependentActions(valuesForMerge);
                foreach (string value in valuesForMerge)
                {
                    switch (value)
                    {
                        case MergeTranslations:
                            targetThesaurus.MergeTranslations(sourceThesaurus);
                            break;
                        case MergeCodes:
                            targetThesaurus.MergeCodes(sourceThesaurus);
                            break;
                        case MergeSynonyms:
                            targetThesaurus.MergeSynonyms(sourceThesaurus);
                            break;
                        case MergeAbbreviations:
                            targetThesaurus.MergeAbbreviations(sourceThesaurus);
                            break;
                        case MergeDefinitions:
                            targetThesaurus.MergeDefinitions(sourceThesaurus);
                            break;
                        default:
                            break;
                    }
                }

                CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(targetThesaurus), userData);
            }
        }

        private void UpdateMergeListIfThereAreDependentActions(List<string> valuesForMerge)
        {
            if (valuesForMerge.Contains(MergeTranslations))
            {
                valuesForMerge.Remove(MergeSynonyms);
                valuesForMerge.Remove(MergeAbbreviations);
                valuesForMerge.Remove(MergeDefinitions);
            }
        }
        #endregion /Merge Thesaurus Values

        #region Merge Thesaurus Occurences
        private int Replace(ThesaurusMerge thesaurusMerge, UserData userData)
        {
            thesaurusMerge.FailedCollections = new List<string>();
            thesaurusMerge.CompletedCollections = new List<string>();

            ReplaceFromForm(thesaurusMerge, userData);
            ReplaceFromFormInstance(thesaurusMerge);
            ReplaceFromFormDistribution(thesaurusMerge);
            ReplaceFromEpisodeOfCare(thesaurusMerge);
            int codesUpdated = ReplaceFromCode(thesaurusMerge);
            int codeSetsUpdated = ReplaceFromCodeSet(thesaurusMerge);

            int? completedStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusMergeState, CodeAttributeNames.Completed);
            int? notCompletedStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusMergeState, CodeAttributeNames.NotCompleted);

            thesaurusMerge.StateCD = thesaurusMerge.FailedCollections.Count == 0 ? completedStateCD : notCompletedStateCD;
            thesaurusMerge.SetLastUpdate();

            return codesUpdated + codeSetsUpdated;
        }
        private void ReplaceFromForm(ThesaurusMerge thesaurusMerge, UserData userData)
        {
            foreach (Form form in formService.GetAll(null))
            {
                form.ReplaceThesauruses(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
                formService.InsertOrUpdate(form, userData);
            }

            if (formService.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormDAL));

            }
        }

        private void ReplaceFromFormInstance(ThesaurusMerge thesaurusMerge)
        {
            formInstanceDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);

            if (formInstanceDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormInstanceDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormInstanceDAL));
            }
        }

        private void ReplaceFromFormDistribution(ThesaurusMerge thesaurusMerge)
        {
            foreach (FormDistribution formDistribution in formDistributionService.GetAll())
            {
                formDistribution.ReplaceThesauruses(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
                formDistributionService.InsertOrUpdate(formDistribution);
            }

            if (formDistributionService.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormDistributionDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormDistributionDAL));
            }
        }

        private void ReplaceFromEpisodeOfCare(ThesaurusMerge thesaurusMerge)
        {
            episodeOfCareDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);

            if (episodeOfCareDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(EpisodeOfCareDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(EpisodeOfCareDAL));
            }
        }

        private int ReplaceFromCode(ThesaurusMerge thesaurusMerge)
        {
            int entriesUpdated = codeDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
            if (codeDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(CodeDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(CodeDAL));
            }

            return entriesUpdated;
        }

        private int ReplaceFromCodeSet(ThesaurusMerge thesaurusMerge)
        {
            int codeSetsUpdated = codeSetDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);

            if (codeSetDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(CodeSetDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(CodeSetDAL));
            }
            return codeSetsUpdated;
        }

        private void SendEmail(UserCookieData userCookieData, DateTime startTime, DateTime endTime, bool noErrors = true)
        {
            string email = userCookieData.Email;
            if (!string.IsNullOrEmpty(email))
            {
                string mailContent = EmailHelpers.GetThesaurusMergeEmailContent(userCookieData, startTime, endTime, noErrors);
                Task.Run(() => emailSender.SendAsync(new EmailDTO(email, mailContent, $"{EmailSenderNames.SoftwareName} Merge Thesaurus Report")));
            }
            
        }
        #endregion /Merge Thesaurus Occurences
    }
}
