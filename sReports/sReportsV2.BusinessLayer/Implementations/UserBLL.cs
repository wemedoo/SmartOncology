using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Constants;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Exceptions;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Cache.Resources;
using sReportsV2.BusinessLayer.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Enums;
using System.Net;
using sReportsV2.Cache.Singleton;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class UserBLL : IUserBLL
    {
        private readonly IPersonnelDAL userDAL;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IEmailSender emailSender;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public UserBLL(IPersonnelDAL userDAL, IOrganizationDAL organizationDAL, IHttpContextAccessor httpContextAccessor, IFormDAL formDAL, IEmailSender emailSender, IMapper mapper, IConfiguration configuration, ICodeDAL codeDAL)
        {
            this.organizationDAL = organizationDAL;
            this.userDAL = userDAL;
            this.httpContextAccessor = httpContextAccessor;
            this.formDAL = formDAL;
            this.emailSender = emailSender;
            this.mapper = mapper;
            this.configuration = configuration;
            this.codeDAL = codeDAL;
        }

        public UserDataOut TryLoginUser(UserLoginDataIn userDataIn)
        {
            UserDataOut result = null;
            Personnel userEntity = IsValidUser(userDataIn);
            int? activeUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Active);
            int? archivedUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            if (userEntity != null)
            {
                UserCookieData userCookieData = mapper.Map<UserCookieData>(userEntity);
                userCookieData.ActiveLanguage = LanguageConstants.EN;
                userCookieData.TimeZoneOffset = userDataIn.TimeZone;
                userEntity.PersonnelConfig.TimeZoneOffset = userDataIn.TimeZone;

                UpdateUserCookieLanguageData(userCookieData);
                result = mapper.Map<UserDataOut>(userCookieData);
                result.Organizations = mapper.Map<List<UserOrganizationDataOut>>(userEntity.GetNonArchivedOrganizations(archivedUserStateCD));

                userDAL.Save();

                result.HasToChooseActiveOrganization = result.IsCurrentSelectedOrganizationNotActive(userCookieData.ActiveOrganization, activeUserStateCD);
            }

            return result;
        }

        public Personnel IsValidUser(UserLoginDataIn userDataIn)
        {
            Personnel userEntity = this.userDAL.GetByUsername(userDataIn.Username);
            if (userEntity != null && userDAL.IsValidUser(userDataIn.Username, PasswordHelper.Hash(userDataIn.Password, userEntity.Salt)))
            {
                return userEntity;
            }

            return null;
        }

        #region CRUD
        public UserDataOut GetById(int userId)
        {
            return mapper.Map<UserDataOut>(userDAL.GetById(userId));
        }

        public async Task<CreateUserResponseResult> InsertOrUpdate(UserDataIn userDataIn, string activeLanguage)
        {
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));
            ValidateUniqueness(userDataIn);

            Personnel user = mapper.Map<Personnel>(userDataIn);
            user.IsDoctor = CheckIsDoctor(user.PersonnelOccupation);
            Personnel userDb = userDAL.GetById(userDataIn.Id) ?? user;
            string passwordForQuickRegistration = null;

            if (userDataIn.Id == 0)
            {
                userDb.PersonnelConfig = new PersonnelConfig() { ActiveLanguage = activeLanguage };
                string generatedPassword = SetCredentialsForNewUser(userDb);
                if (userDb.Email != null)
                {
                    string mailContent = EmailHelpers.GetRegistrationEmailContent(userDb, generatedPassword);
                    await Task.Run(() => emailSender.SendAsync(new EmailDTO(userDb.Email, mailContent, $"{EmailSenderNames.SoftwareName} Registration")));
                }
                else
                {
                    passwordForQuickRegistration = generatedPassword;
                }
            }
            else
            {
                userDb.Copy(user);
            }

            await userDAL.CommitUserTransaction(userDb, mapper.Map<PersonnelOccupation>(userDataIn.PersonnelOccupation));

            return new CreateUserResponseResult()
            {
                Id = userDb.PersonnelId,
                RowVersion = userDb.RowVersion,
                Password = passwordForQuickRegistration,
                Message = userDb.PersonnelId == 0 ? TextLanguage.UserAdministrationMsgCreate : TextLanguage.UserAdministrationMsgEdit
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PersonnelIdentifierDataIn childDataIn)
        {
            PersonnelIdentifier personnelIdentifier = mapper.Map<PersonnelIdentifier>(childDataIn);
            PersonnelIdentifier personnelIdentifierDb = await userDAL.GetById(new QueryEntityParam<PersonnelIdentifier>(childDataIn.Id)).ConfigureAwait(false);

            if (personnelIdentifierDb == null)
            {
                personnelIdentifierDb = personnelIdentifier;
            }
            else
            {
                personnelIdentifierDb.Copy(personnelIdentifier);
            }
            await userDAL.InsertOrUpdate(personnelIdentifierDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = personnelIdentifierDb.PersonnelIdentifierId.ToString(),
                RowVersion = Convert.ToBase64String(personnelIdentifierDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PersonnelAddressDataIn addressDataIn)
        {
            PersonnelAddress personnelAddress = mapper.Map<PersonnelAddress>(addressDataIn);
            PersonnelAddress personnelAddressDb = await userDAL.GetById(new QueryEntityParam<PersonnelAddress>(addressDataIn.Id)).ConfigureAwait(false);

            if (personnelAddressDb == null)
            {
                personnelAddressDb = personnelAddress;
            }
            else
            {
                personnelAddressDb.Copy(personnelAddress);
            }
            await userDAL.InsertOrUpdate(personnelAddressDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = personnelAddressDb.PersonnelAddressId.ToString(),
                RowVersion = Convert.ToBase64String(personnelAddressDb.RowVersion)
            };
        }

        public async Task Delete(PersonnelIdentifierDataIn childDataIn)
        {
            try
            {
                PersonnelIdentifier personnelIdentifier = mapper.Map<PersonnelIdentifier>(childDataIn);
                await userDAL.Delete(personnelIdentifier).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PersonnelAddressDataIn childDataIn)
        {
            try
            {
                PersonnelAddress personnelAddress = mapper.Map<PersonnelAddress>(childDataIn);
                await userDAL.Delete(personnelAddress).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }
        #endregion /CRUD

        public int GetMedicalDoctorsCodeId()
        {
            return SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.OccupationSubCategory).Where(e => e.Thesaurus.Translations
                .Exists(t => t.PreferredTerm == "Medical Doctors")).Select(x => x.Id).FirstOrDefault();
        }

        public List<UserDataOut> GetByIdsList(List<int> ids)
        {
            return mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(ids));
        }

        public bool IsUsernameValid(string username, int? userId)
        {
            return userDAL.IsUsernameValid(username, userId);
        }

        public bool IsEmailValid(string email, int? userId)
        {
            return userDAL.IsEmailValid(email, userId);
        }

        public bool UserExist(int id)
        {
            return userDAL.UserExist(id);
        }

        public PaginationDataOut<UserViewDataOut, DataIn> ReloadTable(PersonnelFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            int? archivedUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            PersonnelFilter filterData = mapper.Map<PersonnelFilter>(dataIn);
            PaginationDataOut<UserViewDataOut, DataIn> result = new PaginationDataOut<UserViewDataOut, DataIn>()
            {
                Count = (int)userDAL.GetAllFilteredCount(filterData, archivedUserStateCD),
                Data = mapper.Map<List<UserViewDataOut>>(userDAL.GetAll(filterData, archivedUserStateCD)),
                DataIn = dataIn
            };

            return result;
        }

        

        public CreateResponseResult UpdateOrganizations(UserDataIn userDataIn)
        {
            Personnel dbUser = userDAL.GetById(userDataIn.Id);
            if (dbUser == null)
            {
                //TO DO THROW EXCEPTION NOT FOUND
            }
            userDAL.UpdateOrganizationsUserCounts(mapper.Map<Personnel>(userDataIn), dbUser);
            if (userDataIn.UserOrganizations != null) 
            {
                foreach (UserOrganizationDataIn userOrganizationDataIn in userDataIn.UserOrganizations)
                {
                    PersonnelOrganization userOrganizationDb = dbUser.Organizations.Find(x => x.OrganizationId == userOrganizationDataIn.OrganizationId);
                    PersonnelOrganization userOrganization = mapper.Map<PersonnelOrganization>(userOrganizationDataIn);
                    if (userOrganizationDb != null)
                    {
                        userOrganizationDb.Copy(userOrganization);
                    }
                    else
                    {
                        dbUser.Organizations.Add(userOrganization);
                        SetUserActiveOrganizationIfNull(dbUser, userOrganizationDataIn.OrganizationId);
                    }
                }
                SetPredefinedFormsForNewUser(dbUser);
            }

            userDAL.InsertOrUpdate(dbUser);
            return new CreateResponseResult()
            {
                Id = dbUser.PersonnelId,
                RowVersion = dbUser.RowVersion,
                Message = TextLanguage.UserAdministrationMsgEdit
            };
        }

        public UserDataOut GetUserForEdit(int userId)
        {
            Personnel dbUser = userDAL.GetById(userId) ?? throw new ArgumentNullException(nameof(userId));
            UserDataOut userData = mapper.Map<UserDataOut>(dbUser);
            return userData;
        }

        public UserOrganizationDataOut LinkOrganization(LinkOrganizationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            OrganizationDataOut organization = mapper.Map<OrganizationDataOut>(organizationDAL.GetById(dataIn.OrganizationId));

            return new UserOrganizationDataOut(organization);
        }

        public void SetState(int id, int? state, int organizationId)
        {
            int? archivedUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            if (state.HasValue)
            {
                userDAL.SetState(id, state, organizationId, archivedUserStateCD);
            }
            else
            {
                userDAL.Delete(id);
            }
        }

        public void SetActiveOrganization(UserCookieData userCookieData, int organizationId)
        {
            Personnel user = this.userDAL.GetById(userCookieData.Id);
            user.PersonnelConfig.ActiveOrganizationId = organizationId;
            this.userDAL.InsertOrUpdate(user);
            userCookieData.UpdateAfterActiveOrganizationChange(user, organizationId);
            UpdateUserCookieInSession(userCookieData);
        }

        public void UpdatePageSize(int pageSize, UserCookieData userCookieData)
        {
            Personnel user = userDAL.GetByUsername(userCookieData.Username);
            if (user != null)
            {
                user.PersonnelConfig.PageSize = pageSize;
            }
            userCookieData.PageSize = pageSize;
            userDAL.InsertOrUpdate(user);
            UpdateUserCookieInSession(userCookieData);
        }

        public void UpdateLanguage(string newLanguage, UserCookieData userCookieData)
        {
            Personnel user = userDAL.GetByUsername(userCookieData.Username);
            if(user != null)
            {
                user.PersonnelConfig.ActiveLanguage = newLanguage;
            }
            userDAL.InsertOrUpdate(user);
            userCookieData.ActiveLanguage = newLanguage;
            UpdateUserCookieInSession(userCookieData);
            UpdateUserCookieLanguageData(userCookieData);
        }

        public void GeneratePassword(string email)
        {
            email = Ensure.IsNotNull(email, nameof(email));
            Personnel user = userDAL.GetByEmail(email);
            var tuplePass = PasswordHelper.CreateHashedPassword(8, user.Salt);
            user.Password = tuplePass.Item2;
            string mailContent = EmailHelpers.GetResetEmailContent(user, tuplePass.Item1);
            Task.Run(() => emailSender.SendAsync(new EmailDTO(user.Email, mailContent, $"{EmailSenderNames.SoftwareName} Reset Password")));
            userDAL.InsertOrUpdate(user);
        }

        public void ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            Personnel user = ValidateChangePasswordInput(oldPassword, newPassword, confirmPassword, userId);
            try
            {
                userDAL.UpdatePassword(user, PasswordHelper.Hash(newPassword, user.Salt));
            }
            catch (Exception)
            {
                string message = "Error while updating user password";
                throw new UserAdministrationException(StatusCodes.Status409Conflict, message);
            }
        }

        public List<UserData> GetUsersForCommentTag(string searchWord)
        {
            List<Personnel> proposedUsers = userDAL.GetUsersForCommentTag(searchWord);
            return mapper.Map<List<UserData>>(proposedUsers);
        }

        public void AddSuggestedForm(string username, string formId)
        {
            Personnel user = userDAL.GetByUsername(username);
            user.AddSuggestedForm(formId);
            userDAL.InsertOrUpdate(user);
        }

        public void RemoveSuggestedForm(string username, string formId)
        {
            Personnel user = userDAL.GetByUsername(username);
            user.RemoveSuggestedForm(formId);
            userDAL.InsertOrUpdate(user);
        }

        public async Task<AutocompleteResultDataOut> GetAutocompleteData(PersonnelAutocompleteDataIn autocompleteFilterDataIn)
        {
            PersonnelAutocompleteFilter personnelAutocompleteFilter = mapper.Map<PersonnelAutocompleteFilter>(autocompleteFilterDataIn);
            personnelAutocompleteFilter.ArchivedUserStateId = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            List<AutoCompleteUserData> users = await userDAL.FilterForAutocomplete(personnelAutocompleteFilter).ConfigureAwait(false);

            return new AutocompleteResultDataOut() { results = mapper.Map<List<AutocompleteDataOut>>(users) };
        }


        public bool ExistEntity(PersonnelIdentifierDataIn dataIn)
        {
            PersonnelIdentifier identifier = mapper.Map<PersonnelIdentifier>(dataIn);
            bool result = userDAL.ExistIdentifier(identifier);
            return result;
        }

        private string SetCredentialsForNewUser(Personnel user)
        {
            user.Salt = PasswordHelper.CreateSalt(10);
            var tuplePass = PasswordHelper.CreateHashedPassword(8, user.Salt);
            user.Password = tuplePass.Item2;

            return tuplePass.Item1;
        }

        private void ValidateUniqueness(UserDataIn userDataIn)
        {
            if (!string.IsNullOrEmpty(userDataIn.Email) && (!userDAL.IsEmailValid(userDataIn.Email, userDataIn.Id) || !userDAL.IsUsernameValid(userDataIn.Username, userDataIn.Id)))
            {
                throw new UserAdministrationException((int)HttpStatusCode.Conflict, $"User with the given data (email: {userDataIn.Email}, username: {userDataIn.Username}) already exist");
            }
        }

        private Personnel ValidateChangePasswordInput(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            if (!Int32.TryParse(userId, out int userIdentifier))
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, "User id is in invalid format.");
            }

            Personnel user = userDAL.GetWithHistoryById(userIdentifier) ?? throw new UserAdministrationException(StatusCodes.Status404NotFound, string.Format("User with given id ({0}) does not exist.", userId));
            if (!PasswordHelper.Hash(oldPassword, user.Salt).Equals(user.Password))
            {
                throw new UserAdministrationException(StatusCodes.Status400BadRequest, "Current password is not correct.");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, "Invalid input: Password should not be empty.");
            }

            if (!newPassword.Equals(confirmPassword))
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, "New and confirm passwords do not match.");
            }

            if (newPassword.Equals(oldPassword))
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, "Old and new password match. Please provide new value.");
            }

            string newPasswordHash = PasswordHelper.Hash(newPassword, user.Salt);
            if (user.PasswordHistory != null && user.PasswordHistory.Contains(newPasswordHash))
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, "You cannot reuse an old password.");
            }

            string errorMessage = PasswordHelper.AdditionalPasswordChecking(newPassword, configuration);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new UserAdministrationException(StatusCodes.Status409Conflict, errorMessage);
            }

            return user;
        }

        private void SetUserActiveOrganizationIfNull(Personnel user, int activeOrganizationId)
        {
            if (user.PersonnelConfig.ActiveOrganizationId == null)
            {
                user.PersonnelConfig.ActiveOrganizationId = activeOrganizationId;
            }
        }

        private void SetPredefinedFormsForNewUser(Personnel user)
        {
            List<string> formRefs = InitializeFormRefs(user);
            if (formRefs.Count > 5)
            {
                Random rnd = new Random();
                for (int i = 1; i <= 5; i++)
                {
                    int index = rnd.Next(formRefs.Count);
                    user.AddSuggestedForm(formRefs[index]);
                }
            }
            else
            {
                user.PersonnelConfig.PredefinedForms = formRefs;
            }
        }

        private List<string> InitializeFormRefs(Personnel user)
        {
            return formDAL.GetByClinicalDomains(organizationDAL.GetClinicalDomainsForIds(user.GetOrganizationRefs()));
        }

        private void UpdateUserCookieInSession(UserCookieData userCookieData)
        {
            var context = httpContextAccessor.HttpContext;
            context.Session.SetObjectAsJson("userData", userCookieData);
        }

        private void UpdateUserCookieLanguageData(UserCookieData userCookieData)
        {
            userCookieData = Ensure.IsNotNull(userCookieData, nameof(userCookieData));

            if (userCookieData.ActiveLanguage != null)
            {
                Thread.CurrentThread.UpdateLanguage(userCookieData.ActiveLanguage);
            }

            var context = httpContextAccessor.HttpContext;
            context.Response.Cookies.Append("Language", userCookieData.ActiveLanguage);
        }

        private bool CheckIsDoctor(PersonnelOccupation occupation)
        {
            if (occupation != null && occupation.OccupationSubCategoryCD == GetMedicalDoctorsCodeId())
                return true;

            return false;
        }
    }
}
