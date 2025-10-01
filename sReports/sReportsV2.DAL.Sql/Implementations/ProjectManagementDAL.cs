using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using sReportsV2.SqlDomain.Helpers;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ProjectManagementDAL : IProjectManagementDAL
    {
        private readonly SReportsContext context;
        public ProjectManagementDAL(SReportsContext context)
        {
            this.context = context;
        }

        public async Task<int> AddDocumentToProject(ProjectDocumentRelation projectDocumentRelation)
        {
            int result = 0;

            if (await IsFormNotInProject(projectDocumentRelation))
            {
                context.ProjectDocumentRelations.Add(projectDocumentRelation);
                result = await context.SaveChangesAsync();
            }
            return result;
        }

        public async Task<int> AddPatientToProjects(List<ProjectPatientRelation> projectPatientRelations)
        {
            int result = 0;
            List<int> projectIds = projectPatientRelations.Select(p => p.ProjectId.GetValueOrDefault()).ToList();

            List<Project> dbProjects = await context.Projects
                .WhereEntriesAreActive()
                .Where(x => projectIds.Contains(x.ProjectId))
                .ToListAsync()
                .ConfigureAwait(false);

            dbProjects.ForEach(x =>
            {
                ProjectPatientRelation projectPatientRelation = projectPatientRelations.Find(p => p.ProjectId == x.ProjectId);
                if (projectPatientRelation != null)
                {
                    x.ProjectPatientRelations.Add(projectPatientRelation);
                }
            });

            result = await context.SaveChangesAsync();

            return result;
        }

        public async Task<int> AddPersonnels(List<ProjectPersonnelRelation> personnelProjects)
        {
            int result = 0;
            int? projectId = personnelProjects.FirstOrDefault()?.ProjectId;

            Project dbProject = context.Projects
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.ProjectId == projectId);

            if (dbProject != null)
            {
                foreach (ProjectPersonnelRelation personnelProject in personnelProjects)
                {
                    dbProject.AddPersonnel(personnelProject);
                }
                result = await context.SaveChangesAsync();

            }
            return result;
        }

        public Task<int> Archive(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Delete(int id)
        {
            Project dbProject = context.Projects.FirstOrDefault(x => x.ProjectId == id);
            dbProject.Delete();

            return await context.SaveChangesAsync();
        }

        public async Task<List<Project>> GetAllAsync(ProjectFilter filter)
        {
            IQueryable<Project> query = GetProjectsFiltered(filter);
            query = GetProjectsOrderedByFilter(filter, query);
            query = ApplyPagingByFilter(filter, query);
            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Project>> GetAllUserProjectsAsync(ProjectFilter filter, int personnelId)
        {
            IQueryable<Project> query = GetUserProjectsFiltered(filter, personnelId);
            query = GetProjectsOrderedByFilter(filter, query);
            query = ApplyPagingByFilter(filter, query);
            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async Task<int> GetAllEntriesCountAsync(ProjectFilter filter)
        {
            return await GetProjectsFiltered(filter).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetAllUserProjectsCountAsync(ProjectFilter filter, int personnelId)
        {
            return await GetUserProjectsFiltered(filter, personnelId).CountAsync().ConfigureAwait(false);
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            return await context.Projects
                .WhereEntriesAreActive()
                .FirstOrDefaultAsync(x => x.ProjectId == id);
        }

        public Project GetById(int id)
        {
            return context.Projects
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.ProjectId == id);
        }

        public Task<List<string>> GetFormIdsByProject(int projectId)
        {
            return context.ProjectDocumentRelations
                .WhereEntriesAreActive()
                .Where(x => x.ProjectId == projectId)
                .Select(x => x.FormId)
                .ToListAsync();
        }

        public List<Project> GetProjectByIds(List<int> ids)
        {
            return context.Projects
                .WhereEntriesAreActive()
                .Where(project => ids.Contains(project.ProjectId)).ToList();
        }

        public async Task<List<int>> GetProjectIdsByPatient(int patientId)
        {
            var projectIds = await context.ProjectPatientRelations
                .WhereEntriesAreActive()
                .Where(x => x.PatientId == patientId)
                .Select(x => x.ProjectId)
                .ToListAsync()
                .ConfigureAwait(false);

            List<int> nonNullableProjectIds = projectIds.Where(x => x.HasValue).Select(x => x.Value).ToList();

            return nonNullableProjectIds;
        }

        public async Task<PaginationData<AutoCompleteData>> GetProjectAutoCompleteTitleAndCount(ProjectFilter filter)
        {
            IQueryable<Project> query = context.Projects
                .WhereEntriesAreActive()
                .Where(x => string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName.ToLower().Contains(filter.ProjectName.ToLower()));

            int count = await query.CountAsync().ConfigureAwait(false);

            query = GetProjectsOrderedByFilter(filter, query);
            query = ApplyPagingByFilter(filter, query);

            List<AutoCompleteData> result = await query.Select(x => new AutoCompleteData() { Id = x.ProjectId.ToString(), Text = x.ProjectName })
                .ToListAsync().ConfigureAwait(false);

            return new PaginationData<AutoCompleteData>(count, result);
        }

        public async Task<Project> InsertOrUpdate(Project project)
        {
            Project result;

            if (project.ProjectId == 0)
            {
                context.Projects.Add(project);
                result = project;
            }
            else
            {
                Project dbProject = context.Projects.FirstOrDefault(x => x.ProjectId == project.ProjectId);
                dbProject.Copy(project);
                context.UpdateEntryMetadata(dbProject);
                result = dbProject;
            }

            await context.SaveChangesAsync();
            return result;
        }

        public async Task<int> RemoveDocumentFromProject(string formId, int projectId)
        {
            if (!string.IsNullOrWhiteSpace(formId) && projectId > 0)
            {
                List<ProjectDocumentRelation> query = await context.ProjectDocumentRelations
                    .WhereEntriesAreActive()
                    .Where(x => x.ProjectId == projectId && formId == x.FormId)
                    .ToListAsync();

                query.ForEach(x =>
                {
                    x.Delete();
                });
            }
            return await context.SaveChangesAsync();
        }

        public async Task<int> RemovePatientFromProject(int patientId, int projectId)
        {
            if (patientId > 0 && projectId > 0)
            {
                ProjectPatientRelation clinicalTrialPatientRelation = await context.ProjectPatientRelations
                    .WhereEntriesAreActive()
                    .FirstOrDefaultAsync(x => x.ProjectId == projectId && patientId == x.PatientId)
                    .ConfigureAwait(false);

                clinicalTrialPatientRelation.Delete();
            }
            return await context.SaveChangesAsync();
        }

        public async Task<int> RemovePersonnelFromProject(int personnelId, int projectId)
        {
            List<ProjectPersonnelRelation> query = await context.ProjectPersonnelRelations
                .WhereEntriesAreActive()
                .Where(x => x.ProjectId == projectId && personnelId == x.PersonnelId)
                .ToListAsync();

            query.ForEach(x =>
            {
                x.Delete();
            });

            return await context.SaveChangesAsync();
        }

        private async Task<bool> IsFormNotInProject(ProjectDocumentRelation projectDocumentRelation)
        {
            return await context.ProjectDocumentRelations
                .WhereEntriesAreActive()
                .Where(x => x.FormId == projectDocumentRelation.FormId && x.ProjectId == projectDocumentRelation.ProjectId)
                .CountAsync() == 0;
        }

        private IQueryable<Project> GetProjectsFiltered(ProjectFilter filter)
        {
            IQueryable<Project> query = context.Projects
                .WhereEntriesAreActive();

            if (!string.IsNullOrWhiteSpace(filter.ProjectName))
            {
                query = query.Where(x => x.ProjectName.ToLower().Contains(filter.ProjectName.ToLower()));
            }
            if (filter.ProjectType.HasValue)
            {
                query = query.Where(x => x.ProjectTypeCD == filter.ProjectType);
            }

            return query;
        }

        private IQueryable<Project> GetUserProjectsFiltered(ProjectFilter filter, int personnelId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone();
            IQueryable<Project> query = context.Projects
                .WhereEntriesAreActive()
                .Where(x => x.ProjectPersonnelRelations.Any(
                    rel => rel.PersonnelId == personnelId 
                    && rel.EntityStateCD != (int)EntityStateCode.Deleted 
                    && rel.ActiveFrom <= now && now <= rel.ActiveTo
                    )
                );

            if (!string.IsNullOrWhiteSpace(filter.ProjectName))
            {
                query = query.Where(x => x.ProjectName.ToLower().Contains(filter.ProjectName.ToLower()));
            }
            if (filter.ProjectType.HasValue)
            {
                query = query.Where(x => x.ProjectTypeCD == filter.ProjectType);
            }

            return query;
        }

        private IQueryable<Project> GetProjectsOrderedByFilter(ProjectFilter filter, IQueryable<Project> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.ColumnName))
            {
                switch (filter.ColumnName)
                {
                    case AttributeNames.ProjectType:
                        query = query.OrderBy(x => x.ProjectTypeCD)
                            .Skip(filter.GetHowManyElementsToSkip())
                            .Take(filter.PageSize);
                        break;
                    default:
                        query = SortTableHelper.OrderByField(query, filter.ColumnName, filter.IsAscending);
                        break;
                }

            }
            else if(filter.ProjectType.HasValue)
            {
                query = query.Where(x => x.ProjectTypeCD == filter.ProjectType).OrderByDescending(x => x.EntryDatetime);
            }
            else
            {
                query = query.OrderByDescending(x => x.EntryDatetime);
            }
            return query;
        }

        private IQueryable<Project> ApplyPagingByFilter(ProjectFilter filter, IQueryable<Project> query)
        {
            return query
                .Skip(filter.GetHowManyElementsToSkip())
                .Take(filter.PageSize);
        }

        public string GetNameById(int projectId)
        {
            return context.Projects
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.ProjectId == projectId)?.ProjectName;
        }

        public List<Project> GetAllByIds(List<int?> ids)
        {
            return context.Projects.Where(x => ids.Contains(x.ProjectId)).ToList();
        }

        public List<int> GetAllProjectsIdsFor(int personnedId)
        {
            return context.ProjectPersonnelRelations
                .WhereEntriesAreActive()
                .Where(ppr => ppr.PersonnelId == personnedId)
                .Select(ppr => ppr.ProjectId)
                .AsEnumerable()
                .Select(projectId => projectId.GetValueOrDefault())
                .Where(projectId => projectId != 0)
                .ToList();
        }
    }
}
