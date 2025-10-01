using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IUserBLL : IExistBLL<PersonnelIdentifierDataIn>, IChildEntryBLL<PersonnelIdentifierDataIn>, IChildEntryBLL<PersonnelAddressDataIn>
    {
        UserDataOut TryLoginUser(UserLoginDataIn user);
        Personnel IsValidUser(UserLoginDataIn user);
        UserDataOut GetUserForEdit(int userId);
        void GeneratePassword(string email);
        PaginationDataOut<UserViewDataOut, DataIn> ReloadTable(PersonnelFilterDataIn dataIn);
        Task<CreateUserResponseResult> InsertOrUpdate(UserDataIn userDataIn, string activeLanguage);
        CreateResponseResult UpdateOrganizations(UserDataIn userDataIn);
        void UpdateLanguage(string newLanguage, UserCookieData userCookieData);
        UserOrganizationDataOut LinkOrganization(LinkOrganizationDataIn dataIn);
        UserDataOut GetById(int userId);
        List<UserDataOut> GetByIdsList(List<int> ids);
        bool IsUsernameValid(string username, int? userId);
        bool IsEmailValid(string email, int? userId);
        bool UserExist(int id);
        void SetState(int id, int? state, int organizationId);
        void SetActiveOrganization(UserCookieData userCookieData, int organizationId);
        void UpdatePageSize(int pageSize, UserCookieData userCookieData);
        void ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId);
        List<UserData> GetUsersForCommentTag(string searchWord);
        void AddSuggestedForm(string username, string formId);
        void RemoveSuggestedForm(string username, string formId);
        Task<AutocompleteResultDataOut> GetAutocompleteData(PersonnelAutocompleteDataIn autocompleteFilterDataIn);
        int GetMedicalDoctorsCodeId();
    }
}
