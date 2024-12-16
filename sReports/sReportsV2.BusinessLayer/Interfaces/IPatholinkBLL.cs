using sReports.PathoLink.Entities;
using sReportsV2.DTOs.User.DTO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPatholinkBLL
    {
        bool Import(PathoLink pathoLink, UserCookieData userCookieData);
        Task<PathoLink> Export(string formInstanceId, UserCookieData userCookieData);
    }
}
