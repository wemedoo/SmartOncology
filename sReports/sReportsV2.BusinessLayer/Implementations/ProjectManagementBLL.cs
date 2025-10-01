using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ClinicalTrial;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataIn;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataOut;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ProjectManagementBLL : IProjectManagementBLL
    {
        private readonly IProjectManagementDAL projectManagementDAL;
        private readonly IPersonnelDAL personnelDAL;
        private readonly IFormBLL formBLL;
        private readonly ITrialManagementDAL trialManagementDAL;
        private readonly IMapper mapper;
        private readonly ICodeDAL codeDAL;

        public ProjectManagementBLL(IProjectManagementDAL projectManagementDAL, IPersonnelDAL personnelDAL, IFormBLL formBLL, ITrialManagementDAL trialManagementDAL, ICodeDAL codeDAL, IMapper mapper)
        {
            this.projectManagementDAL = projectManagementDAL;
            this.personnelDAL = personnelDAL;
            this.formBLL = formBLL;
            this.trialManagementDAL = trialManagementDAL;
            this.codeDAL = codeDAL;
            this.mapper = mapper;
        }

        public async Task<int> AddDocumentToProject(ProjectDocumentDataIn dataIn)
        {
            int result = 0;

            if (!string.IsNullOrWhiteSpace(dataIn.FormId) && dataIn.ProjectId > 0)
            {
                result = await projectManagementDAL
                    .AddDocumentToProject(mapper.Map<ProjectDocumentRelation>(dataIn))
                    .ConfigureAwait(false);
            }

            return result;
        }

        public async Task<int> AddPatientToProjects(List<PatientProjectDataIn> dataIn)
        {
            int result = 0;

            if (dataIn.Count > 0)
            {
                result = await projectManagementDAL
                    .AddPatientToProjects(mapper.Map<List<ProjectPatientRelation>>(dataIn))
                    .ConfigureAwait(false);
            }

            return result;
        }

        public async Task<int> AddPersonnelsToProject(List<ProjectPersonnelDataIn> personnelProjects)
        {
            int result = 0;

            if (personnelProjects.Count > 0)
            {
                result = await projectManagementDAL
                    .AddPersonnels(mapper.Map<List<ProjectPersonnelRelation>>(personnelProjects))
                    .ConfigureAwait(false);
            }

            return result;
        }

        public Task<int> Archive(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;
            try
            {
                result = await projectManagementDAL.Delete(id).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
            return result;
        }

        public async Task<ProjectDataOut> GetById(int id)
        {
            if (id > 0)
            {
                ProjectDataOut projectDataOut = mapper.Map<ProjectDataOut>(await projectManagementDAL.GetByIdAsync(id).ConfigureAwait(false));
                ClinicalTrialDataOut clinicalTrial = mapper.Map<ClinicalTrialDataOut>(await trialManagementDAL.GetByProjectId(projectDataOut.ProjectId).ConfigureAwait(false));
                projectDataOut.ClinicalTrial = clinicalTrial != null ? clinicalTrial : new ClinicalTrialDataOut();
                projectDataOut.Personnels = new List<UserDataOut>();

                return projectDataOut;
            }
            return null;
        }

        public async Task<AutocompleteResultDataOut> GetDocumentTitleForAutocomplete(AutocompleteDataIn dataIn, UserCookieData userCookieData, int projectId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            Task<List<string>> getFormIdsByProjectTask = projectManagementDAL.GetFormIdsByProject(projectId);
            Task<AutocompleteResultDataOut> getTitleDataForAutocompleteTask = formBLL.GetTitleDataForAutocomplete(dataIn, userCookieData);

            await Task.WhenAll(getFormIdsByProjectTask, getTitleDataForAutocompleteTask);

            List<string> formIdsInProject = getFormIdsByProjectTask.Result;
            AutocompleteResultDataOut documentsIdsAndTitles = getTitleDataForAutocompleteTask.Result;

            RemoveIdsToExcludeFromAutocomplete(documentsIdsAndTitles, formIdsInProject);
            return documentsIdsAndTitles;
        }

        public string GetNameById(int projectId)
        {
            return projectManagementDAL.GetNameById(projectId);
        }

        public async Task<AutocompleteResultDataOut> GetProjectAutoCompleteTitle(AutocompleteDataIn dataIn, int clinicalTrialTypeId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ProjectFilter filter = new ProjectFilter() { ProjectName = dataIn.Term, ProjectType = clinicalTrialTypeId, Page = dataIn.Page, PageSize = FilterConstants.DefaultPageSize };
            PaginationData<AutoCompleteData> projectsAndCount = await projectManagementDAL.GetProjectAutoCompleteTitleAndCount(filter);

            List<AutocompleteDataOut> autocompleteDataDataOuts = projectsAndCount.Data
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id,
                    text = x.Text,
                })
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = autocompleteDataDataOuts,
                pagination = new AutocompletePaginatioDataOut() { more = dataIn.ShouldLoadMore(projectsAndCount.Count) }
            };

            return result;
        }

        public async Task<AutocompleteResultDataOut> GetProjectAutoCompleteTitleForPatient(AutocompleteDataIn dataIn, int patientId, int clinicalTrialTypeId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            List<int> patientProjectIds = await projectManagementDAL.GetProjectIdsByPatient(patientId).ConfigureAwait(false);
            AutocompleteResultDataOut result = await GetProjectAutoCompleteTitle(dataIn, clinicalTrialTypeId).ConfigureAwait(false);

            RemoveIdsToExcludeFromAutocomplete(result, patientProjectIds.Select(x => x.ToString()));
            return result;
        }

        public async Task<ProjectDataOut> InsertOrUpdate(ProjectDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            Project project = mapper.Map<Project>(dataIn);
            var projectDataOut = mapper.Map<ProjectDataOut>(await projectManagementDAL.InsertOrUpdate(project));

            if (!string.IsNullOrEmpty(dataIn.ClinicalTrial.ClinicalTrialTitle))
            {
                dataIn.ClinicalTrial.ProjectId = projectDataOut.ProjectId;
                projectDataOut.ClinicalTrial = mapper.Map<ClinicalTrialDataOut>(await trialManagementDAL.InsertOrUpdate(mapper.Map<ClinicalTrial>(dataIn.ClinicalTrial)));
            }
            else if (dataIn.ClinicalTrial.ClinicalTrialId != 0) 
            {
                await trialManagementDAL.Delete(dataIn.ClinicalTrial.ClinicalTrialId).ConfigureAwait(false);
                projectDataOut.ClinicalTrial = new ClinicalTrialDataOut();
            }

            return projectDataOut;
        }

        public async Task<PaginationDataOut<FormDataOut, FormFilterDataIn>> ReloadDocumentsTable(FormFilterDataIn dataIn, int projectId, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PaginationDataOut<FormDataOut, FormFilterDataIn> result = null;

            if (projectId > 0)
            {
                List<string> ids = await projectManagementDAL.GetFormIdsByProject(projectId).ConfigureAwait(false);
                if (ids.Count > 0)
                {
                    dataIn.Ids = ids;
                    result = formBLL.ReloadData(dataIn, userCookieData);
                }
            }
            return result;
        }

        public async Task<PaginationDataOut<UserDataOut, DataIn>> ReloadPersonnelTable(ProjectFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ProjectFilter filterData = mapper.Map<ProjectFilter>(dataIn);
            int? archivedUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            PaginationData<Personnel> personnelsAndCount = await personnelDAL.FilterAndCountPersonnelByProject(filterData, archivedUserStateCD).ConfigureAwait(false);

            PaginationDataOut<UserDataOut, DataIn> result = new PaginationDataOut<UserDataOut, DataIn>()
            {
                Count = personnelsAndCount.Count,
                Data = mapper.Map<List<UserDataOut>>(personnelsAndCount.Data),
                DataIn = dataIn
            };
            return result;
        }

        public async Task<PaginationDataOut<ProjectDataOut, DataIn>> ReloadTable(ProjectFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ProjectFilter filterData = mapper.Map<ProjectFilter>(dataIn);

            PaginationDataOut<ProjectDataOut, DataIn> result = new PaginationDataOut<ProjectDataOut, DataIn>()
            {
                Count = await projectManagementDAL.GetAllEntriesCountAsync(filterData).ConfigureAwait(false),
                Data = mapper.Map<List<ProjectDataOut>>(await projectManagementDAL.GetAllAsync(filterData).ConfigureAwait(false)),
                DataIn = dataIn
            };

            return result;
        }

        public async Task<PaginationDataOut<ProjectDataOut, DataIn>> ReloadUserProjectTable(ProjectFilterDataIn dataIn, int personnelId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ProjectFilter filterData = mapper.Map<ProjectFilter>(dataIn);

            PaginationDataOut<ProjectDataOut, DataIn> result = new PaginationDataOut<ProjectDataOut, DataIn>()
            {
                Count = await projectManagementDAL.GetAllUserProjectsCountAsync(filterData, personnelId).ConfigureAwait(false),
                Data = mapper.Map<List<ProjectDataOut>>(await projectManagementDAL.GetAllUserProjectsAsync(filterData, personnelId).ConfigureAwait(false)),
                DataIn = dataIn
            };

            return result;
        }

        public async Task<int> RemoveDocumentFromProject(string formId, int projectId)
        {
            int result = 0;

            if (!string.IsNullOrWhiteSpace(formId) && projectId > 0)
            {
                result = await projectManagementDAL.RemoveDocumentFromProject(formId, projectId).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<int> RemovePatientFromProject(int patientId, int projectId)
        {
            int result = 0;

            if (patientId > 0 && projectId > 0)
            {
                result = await projectManagementDAL.RemovePatientFromProject(patientId, projectId).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<int> RemovePersonnelsFromProject(int personnelId, int projectId)
        {
            int result = 0;

            if (personnelId > 0 && projectId > 0)
            {
                result = await projectManagementDAL.RemovePersonnelFromProject(personnelId, projectId).ConfigureAwait(false);
            }

            return result;
        }

        private void RemoveIdsToExcludeFromAutocomplete(AutocompleteResultDataOut autoCompleteResult, IEnumerable<string> excludedIds)
        {
            autoCompleteResult.results.RemoveAll(x => excludedIds.Contains(x.id));
        }
    }
}
