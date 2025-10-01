using DocumentFormat.OpenXml.Office2010.ExcelAc;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.MongoDb.Entities.Promp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.DAL.MongoDb.Interfaces
{
    public interface IPromptConfigurationDAL
    {
        Task<List<PromptForm>> GetAll();
        Task<PromptForm> GetPromptForm(PromptFormFilter promptFormFilter);
        Task<PromptFormVersion> GetPromptFormVersion(PromptFormFilter promptFormFilter);
        Task<string> GetPrompt(PromptFormFilter promptFormFilter);
        Task<List<Version>> GetVersions(PromptFormFilter promptFormFilter);
        Task InsertOrUpdate(PromptForm promptForm);
    }
}
