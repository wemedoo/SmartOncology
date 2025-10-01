using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.MongoDb.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.MongoDb.Entities.Promp;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Prompt.DataIn;
using sReportsV2.DTOs.DTOs.Prompt.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Version = sReportsV2.Domain.Entities.Form.Version;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PromptConfigurationBLL : IPromptConfigurationBLL
    {
        private readonly IProjectManagementDAL projectManagementDAL;
        private readonly IFormDAL formDAL;
        private readonly IPersonnelDAL personnelDAL;
        private readonly IPromptConfigurationDAL promptConfigurationDAL;
        private readonly IMapper mapper;

        public PromptConfigurationBLL(IProjectManagementDAL projectManagementDAL, IFormDAL formDAL, IPromptConfigurationDAL promptConfigurationDAL, IMapper mapper, IPersonnelDAL personnelDAL)
        {
            this.projectManagementDAL = projectManagementDAL;
            this.formDAL = formDAL;
            this.promptConfigurationDAL = promptConfigurationDAL;
            this.mapper = mapper;
            this.personnelDAL = personnelDAL;
        }

        public async Task<PromptDataOut> GetFormPrompt(PromptDataIn dataIn)
        {
            PromptForm promptForm = await promptConfigurationDAL.GetPromptForm(mapper.Map<PromptFormFilter>(dataIn));
            return new PromptDataOut
            {
                Form = await GetForm(dataIn.FormId).ConfigureAwait(false),
                ProjectId = dataIn.ProjectId,
                ProjectName = projectManagementDAL
                    .GetProjectByIds(new List<int> { dataIn.ProjectId })
                    .FirstOrDefault().ProjectName,
                CurrentVersionId = promptForm?.CurrentVersionId,
                LatestVersion = promptForm == null || promptForm.GetPromptFormVersion(string.Empty).Version.Id == promptForm?.CurrentVersionId,
                Versions = mapper.Map<List<VersionDTO>>(GetVersions(promptForm))
            };
        }

        public async Task<PromptDetailDataOut> GetPrompt(PromptDataIn dataIn)
        {
            FormDataOut formDataOut = await GetForm(dataIn.FormId).ConfigureAwait(false);
            FieldDataOut fieldDataOut = formDataOut.GetAllFields().Find(f => f.Id == dataIn.FieldId);

            return new PromptDetailDataOut
            {
                Prompt = await promptConfigurationDAL.GetPrompt(mapper.Map<PromptFormFilter>(dataIn)),
                Field = fieldDataOut,
                FormName = formDataOut.Title
            };
        }

        public async Task<string> UpdatePrompt(PromptInputDataIn dataIn, int userId)
        {
            string versionId = string.Empty;
            PromptForm promptFormDb = await promptConfigurationDAL.GetPromptForm(mapper.Map<PromptFormFilter>((PromptDataIn)dataIn));
            if (promptFormDb == null)
            {
                versionId = Guid.NewGuid().ToString();
                promptFormDb = new PromptForm(dataIn.FormId, dataIn.ProjectId, dataIn.FieldId, versionId, dataIn.Prompt, userId);
            }
            else
            {
                promptFormDb.UpdatePrompt(dataIn.VersionId, dataIn.FieldId, dataIn.Prompt);
            }
            await promptConfigurationDAL.InsertOrUpdate(promptFormDb);
            return versionId;
        }

        public async Task<VersionDTO> AddNewPromptVersion(PromptDataIn dataIn, int userId)
        {
            PromptForm promptFormDb = await promptConfigurationDAL.GetPromptForm(mapper.Map<PromptFormFilter>(dataIn));
            VersionDTO version = mapper.Map<VersionDTO>(promptFormDb.AddNewVersion(userId));
            await promptConfigurationDAL.InsertOrUpdate(promptFormDb);  
            return version;
        }

        public async Task<bool> SwitchPromptVersion(PromptDataIn dataIn)
        {
            PromptForm promptFormDb = await promptConfigurationDAL.GetPromptForm(mapper.Map<PromptFormFilter>(dataIn));
            bool latestVersion = promptFormDb.GetPromptFormVersion(string.Empty).Version.Id == dataIn.VersionId;
            promptFormDb.CurrentVersionId = dataIn.VersionId;
            await promptConfigurationDAL.InsertOrUpdate(promptFormDb);
            return latestVersion;
        }

        public async Task<PromptFormVersionDataOut> PreviewPrompts(PromptDataIn dataIn)
        {
            PromptFormVersion promptFormVersion = await promptConfigurationDAL.GetPromptFormVersion(mapper.Map<PromptFormFilter>(dataIn));
            FormDataOut form = await GetForm(dataIn.FormId);
            PromptFormVersionDataOut promptFormVersionData = null;
            if (promptFormVersion != null)
            {
                promptFormVersionData = mapper.Map<PromptFormVersionDataOut>(promptFormVersion);
                promptFormVersionData.FormName = form.Title;
                promptFormVersionData.CreatedBy = mapper.Map<UserShortInfoDataOut>(personnelDAL.GetById(promptFormVersion.CreatedById));
                promptFormVersionData.PreparePrompts(form);
            }

            return promptFormVersionData;
        }

        private async Task<FormDataOut> GetForm(string formId)
        {
            Form form = await formDAL.GetFormAsync(formId).ConfigureAwait(false);
            return mapper.Map<FormDataOut>(form);
        }

        public List<Version> GetVersions(PromptForm promptForm)
        {
            return promptForm?.PromptFormVersions?.Select(x => x.Version)?.ToList()
                ?? new List<Version>() {
                    new Version {
                        Major = 1
                    }
            };
        }
    }
}
