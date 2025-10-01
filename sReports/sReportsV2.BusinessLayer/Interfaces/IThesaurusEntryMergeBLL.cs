using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.User.DTO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IThesaurusEntryMergeBLL
    {
        void TakeBoth(int currentId);
        Task MergeThesauruses(ThesaurusMergeDataIn thesaurusMergeDataIn, UserData userData);
        Task<int> MergeThesaurusOccurences(UserCookieData userCookieData);
    }
}
