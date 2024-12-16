using sReportsV2.DTOs.Common.DTO;

namespace sReportsV2.BusinessLayer.Components.Interfaces
{
    public interface IEmailSender
    {
        void SendAsync(EmailDTO messageDto);

    }
}
