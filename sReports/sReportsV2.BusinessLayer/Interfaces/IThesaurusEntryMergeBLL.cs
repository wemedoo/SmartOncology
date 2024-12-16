using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.User.DTO;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IThesaurusEntryMergeBLL
    {
        void TakeBoth(int currentId);
        void MergeThesauruses(ThesaurusMergeDataIn thesaurusMergeDataIn, UserData userData);
        int MergeThesaurusOccurences(UserCookieData userCookieData);
    }
}
