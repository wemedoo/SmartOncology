using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Prompt.DataIn;
using sReportsV2.DTOs.DTOs.Prompt.DataOut;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPromptConfigurationBLL
    {
        Task<PromptDataOut> GetFormPrompt(PromptDataIn dataIn);
        Task<PromptFormVersionDataOut> PreviewPrompts(PromptDataIn dataIn);
        Task<PromptDetailDataOut> GetPrompt(PromptDataIn dataIn);
        Task<string> UpdatePrompt(PromptInputDataIn dataIn, int userId);
        Task<VersionDTO> AddNewPromptVersion(PromptDataIn dataIn, int userId);
        Task<bool> SwitchPromptVersion(PromptDataIn dataIn);
    }
}
