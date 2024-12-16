using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.Organization;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.Common.Extensions;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Organization.DataOut;
using sReportsV2.DTOs.DTOs.Organization.DataIn;
using System.Threading.Tasks;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class OrganizationBLL : IOrganizationBLL
    {
        private readonly IOrganizationDAL organizationDAL;
        private readonly IOrganizationRelationDAL organizationRelationDAL;
        private readonly IPersonnelDAL personnelDAL;
        private readonly IMapper Mapper;

        public OrganizationBLL(IOrganizationDAL organizationDAL, IOrganizationRelationDAL organizationRelationDAL, IPersonnelDAL personnelDAL, IMapper mapper)
        {
            this.organizationDAL = organizationDAL;
            this.organizationRelationDAL = organizationRelationDAL;
            this.personnelDAL = personnelDAL;
            Mapper = mapper;
        }

        public PaginationDataOut<OrganizationDataOut, DataIn> ReloadTable(OrganizationFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            OrganizationFilter filterData = Mapper.Map<OrganizationFilter>(dataIn);

            List<Organization> organizationsFiltered = organizationDAL.GetAll(filterData);
            PaginationDataOut<OrganizationDataOut, DataIn> result = new PaginationDataOut<OrganizationDataOut, DataIn>()
            {
                Count = (int) organizationDAL.GetAllFilteredCount(filterData),
                Data = Mapper.Map<List<OrganizationDataOut>>(organizationsFiltered),
                DataIn = dataIn
            };

            return result;
        }

        #region CRUD
        public OrganizationDataOut GetOrganizationById(int organizationId)
        {
            return Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(organizationId));
        }

        public OrganizationDataOut GetOrganizationForEdit(int organizationId)
        {
            Organization organization = organizationDAL.GetById(organizationId);
            return Mapper.Map<OrganizationDataOut>(organization);
        }

        public CreateResponseResult Insert(OrganizationDataIn organizationDataIn)
        {
            try
            {
                Organization organization = Mapper.Map<Organization>(organizationDataIn);
                if (organization.OrganizationId != 0)
                {
                    Organization dbOrganization = organizationDAL.GetById(organization.OrganizationId) ?? throw new KeyNotFoundException();
                    dbOrganization.Copy(organization);
                    organization = dbOrganization;
                }
                organizationDAL.InsertOrUpdate(organization);
                OrganizationRelation organizationRelation = Mapper.Map<OrganizationRelation>(
                    new OrganizationRelationDataIn
                    {
                        ChildId = organization.OrganizationId,
                        ParentId = organizationDataIn.ParentId.GetValueOrDefault()
                    }
                );
                organization.SetRelation(organizationRelation);
                organizationDAL.InsertOrUpdate(organization);


                return new CreateResponseResult { Id = organization.OrganizationId, RowVersion = organization.RowVersion };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("A concurrency error occurred while updating the database. Please try again.", ex);
            }
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(OrganizationIdentifierDataIn identifierDataIn)
        {
            OrganizationIdentifier organizationIdentifier = Mapper.Map<OrganizationIdentifier>(identifierDataIn);
            OrganizationIdentifier organizationIdentifierDb = await organizationDAL.GetById(new QueryEntityParam<OrganizationIdentifier>(identifierDataIn.Id)).ConfigureAwait(false);

            if (organizationIdentifierDb == null)
            {
                organizationIdentifierDb = organizationIdentifier;
            }
            else
            {
                organizationIdentifierDb.Copy(organizationIdentifier);
            }
            await organizationDAL.InsertOrUpdate(organizationIdentifierDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = organizationIdentifierDb.OrganizationIdentifierId.ToString(),
                RowVersion = Convert.ToBase64String(organizationIdentifierDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(OrganizationTelecomDataIn telecomDataIn)
        {
            OrganizationTelecom organizationTelecom = Mapper.Map<OrganizationTelecom>(telecomDataIn);
            OrganizationTelecom organizationTelecomDb = await organizationDAL.GetById(new QueryEntityParam<OrganizationTelecom>(telecomDataIn.Id)).ConfigureAwait(false);

            if (organizationTelecomDb == null)
            {
                organizationTelecomDb = organizationTelecom;
            }
            else
            {
                organizationTelecomDb.Copy(organizationTelecom);
            }
            await organizationDAL.InsertOrUpdate(organizationTelecomDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = organizationTelecomDb.OrganizationTelecomId.ToString(),
                RowVersion = Convert.ToBase64String(organizationTelecomDb.RowVersion)
            };
        }

        public void Delete(OrganizationDataIn organizationDataIn)
        {
            try
            {
                Organization organization = Mapper.Map<Organization>(organizationDataIn);
                organizationDAL.Delete(organization);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(OrganizationIdentifierDataIn organizationIdentifierDataIn)
        {
            try
            {
                OrganizationIdentifier organizationIdentifier = Mapper.Map<OrganizationIdentifier>(organizationIdentifierDataIn);
                await organizationDAL.Delete(organizationIdentifier).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(OrganizationTelecomDataIn childDataIn)
        {
            try
            {
                OrganizationTelecom organizationTelecom = Mapper.Map<OrganizationTelecom>(childDataIn);
                await organizationDAL.Delete(organizationTelecom).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }
        #endregion /CRUD

        public AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            IQueryable<Organization> filtered = organizationDAL.FilterByName(dataIn.Term);
            List<AutocompleteDataOut> organizationDataOuts = filtered.OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.OrganizationId.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = organizationDataOuts
            };

            return result;
        }

        public List<OrganizationDataOut> ReloadHierarchy(int? parentId)
        {
            List<OrganizationDataOut> result = new List<OrganizationDataOut>();
            List<OrganizationRelation> hierarchyList = organizationRelationDAL.GetOrganizationHierarchies();
            LoadHierarchyTree(hierarchyList, parentId.GetValueOrDefault(), result);
            return result;
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries)
        {
            return organizationDAL.GetOrganizationUsersCount(term, MapCountriesToCustomEnum(countries));
        }

        public List<OrganizationUsersDataOut> GetUsersByOrganizationsIds(List<int> ids)
        {
            List<PersonnelOrganization> userOrganizations = GetUsersByOrganizationsWithPermission(organizationDAL.GetUsersByOrganizationIds(ids));
            
            return userOrganizations.GroupBy(x => new { x.OrganizationId, x.Organization.Name }).Select(x => new OrganizationUsersDataOut
            {
                Id = x.Key.OrganizationId,
                Name = x.Key.Name,
                Users = Mapper.Map<List<UserDataOut>>(x.Select(y => y.Personnel).ToList())
            }).ToList();
        }

        public List<OrganizationCommunicationEntityDataOut> GetOrgCommunicationByOrgId(OrganizationFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            OrganizationFilter filterData = Mapper.Map<OrganizationFilter>(dataIn);

            List<OrganizationCommunicationEntity> organizationsFiltered = organizationDAL.GetOrgCommunicationByOrgId(filterData);
            List<OrganizationCommunicationEntityDataOut> result = Mapper.Map<List<OrganizationCommunicationEntityDataOut>>(organizationsFiltered);

            return result;
        }

        public async Task<int> InsertOrganizationCommunication(OrganizationCommunicationEntityDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            OrganizationCommunicationEntity organizationCommunicationEntity = Mapper.Map<OrganizationCommunicationEntity>(dataIn);

            return await organizationDAL.InsertOrganizationCommunication(organizationCommunicationEntity).ConfigureAwait(false);
        }

        public async Task<OrganizationCommunicationEntityDataOut> GetOrgCommunicationEntityIdByIdAsync(int orgCommunicationEntityId)
        {
            OrganizationCommunicationEntity organizationCommunicationEntity = await organizationDAL.GetOrgCommunicationEntityIdByIdAsync(orgCommunicationEntityId);

            return Mapper.Map<OrganizationCommunicationEntityDataOut>(organizationCommunicationEntity);
        }

        private void LoadHierarchyTree(List<OrganizationRelation> hierarchyList, int parentId, List<OrganizationDataOut> hierarchy)
        {
            OrganizationRelation parentHierarchy = hierarchyList.Find(x => x.ChildId == parentId);
            if (parentHierarchy != null)
            {
                var parent = Mapper.Map<OrganizationDataOut>(parentHierarchy.Parent);
                var child = Mapper.Map<OrganizationDataOut>(parentHierarchy.Child);

                if (!hierarchy.Exists(x => x.Id == parent.Id))
                {
                    hierarchy.Add(parent);
                }

                if (!hierarchy.Exists(x => x.Id == child.Id))
                {
                    hierarchy.Add(child);
                }

                LoadHierarchyTree(hierarchyList, parentHierarchy.ParentId, hierarchy);
            }
            else
            {
                Organization organization = organizationDAL.GetById(parentId);
                if (organization != null)
                {
                    var org = Mapper.Map<OrganizationDataOut>(organization);
                    if (!hierarchy.Exists(x => x.Id == org.Id))
                    {
                        hierarchy.Add(org);
                    }
                }
            }
        }

        private List<PersonnelOrganization> GetUsersByOrganizationsWithPermission(List<PersonnelOrganization> userOrganizations)
        {
            List<PersonnelOrganization> userOrganizationsWithPermission = new List<PersonnelOrganization>();

            foreach (PersonnelOrganization userOrganization in userOrganizations)
            {
                if (personnelDAL.UserHasPermission(userOrganization.PersonnelId, ModuleNames.Designer, PermissionNames.FindConsensus))
                {
                    userOrganizationsWithPermission.Add(userOrganization);
                }
            }

            return userOrganizationsWithPermission;
        }

        private Dictionary<int, string> MapCountriesToCustomEnum(List<string> countryNames)
        {
            Dictionary<int, string> countries = new Dictionary<int, string>();
            if (countryNames != null)
            {
                foreach (string countryName in countryNames)
                {
                    int customEnumId = SingletonDataContainer.Instance.GetCodeIdForPreferredTerm(countryName, (int)CodeSetList.Country);
                    if (customEnumId > 0)
                    {
                        countries.Add(customEnumId, countryName);
                    }
                }
            }

            return countries;
        }

        public string GetTimeZoneOffset(int organizationId)
        {
            return organizationDAL.GetTimeZoneOffset(organizationId);
        }

        public bool ExistEntity(OrganizationIdentifierDataIn dataIn)
        {
            OrganizationIdentifier identifier = Mapper.Map<OrganizationIdentifier>(dataIn);
            bool result = organizationDAL.ExistIdentifier(identifier);
            return result;
        }
    }
}
