using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.CodeEntry.DataOut;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PersonnelTeamBLL : IPersonnelTeamBLL
    {
        private readonly IPersonnelTeamDAL personnelTeamDAL;
        private readonly IMapper Mapper;

        public PersonnelTeamBLL(IPersonnelTeamDAL personnelTeamDAL, IMapper mapper)
        {
            this.personnelTeamDAL = personnelTeamDAL;
            Mapper = mapper;
        }

        public PersonnelTeamDataOut GetById(int personnelTeamId)
        {
            PersonnelTeam personnelTeam = personnelTeamDAL.GetByIdJoinActiveRelations(personnelTeamId);
            return Mapper.Map<PersonnelTeamDataOut>(personnelTeam);
        }

        public void InsertOrUpdate(PersonnelTeamDataIn personnelTeamDataIn)
        {
            PersonnelTeam personnelTeam = Mapper.Map<PersonnelTeam>(personnelTeamDataIn);

            if (personnelTeam.PersonnelTeamId != 0)
            {
                PersonnelTeam dbPersonnelTeam = personnelTeamDAL.GetById(personnelTeam.PersonnelTeamId);
                if (dbPersonnelTeam == null) throw new KeyNotFoundException();

                dbPersonnelTeam.CopyData(personnelTeam);
                personnelTeam = dbPersonnelTeam;
            }
            personnelTeamDAL.InsertOrUpdate(personnelTeam);
        }

        public PaginationDataOut<PersonnelTeamDataOut, DataIn> GetAllFiltered(PersonnelTeamFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            PersonnelTeamFilter filter = Mapper.Map<PersonnelTeamFilter>(dataIn);
            PaginationDataOut<PersonnelTeamDataOut, DataIn> result = new PaginationDataOut<PersonnelTeamDataOut, DataIn>()
            {
                Count = personnelTeamDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<PersonnelTeamDataOut>>(personnelTeamDAL.GetAll(filter)),
                DataIn = dataIn
            };
            return result;
        }

        public AutocompleteResultDataOut GetNameForAutocomplete(AutocompleteDataIn autocompleteDataIn, int organizationId)
        {
            autocompleteDataIn = Ensure.IsNotNull(autocompleteDataIn, nameof(autocompleteDataIn));

            IQueryable<PersonnelTeam> filtered = personnelTeamDAL.FilterByName(autocompleteDataIn.Term)
                .Where(x => organizationId == 0 || x.PersonnelTeamOrganizationRelations.Any(y => y.OrganizationId == organizationId))  // if 0 return every Team (PersonnelTeam could be not related to an Org)
                .OrderBy(x => x.Name);

            List<AutocompleteDataOut> personnelTeamDataOuts = filtered
                .AsEnumerable()
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.PersonnelTeamId.ToString(),
                    text = x.Name
                })
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = personnelTeamDataOuts
            };

            return result;
        }

        public bool IsNameUsedCheck(string name, int organizationId, int personnelTeamId)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(name) && organizationId != 0)
            {
                IQueryable<PersonnelTeam> filtered = personnelTeamDAL.FilterByName(name)
                    .Where(x => personnelTeamId == 0 || x.PersonnelTeamId != personnelTeamId)
                    .Where(x => x.PersonnelTeamOrganizationRelations.Any(y => y.OrganizationId == organizationId));

                filtered.ToList().ForEach(x => {
                    if (x.Name == name)
                        result = true;
                });
            }
            return result;
        }

        public AutocompleteResultDataOut GetTeamTypeCodes(string activeLanguage, List<CodeDataOut> CodesDataOut)
        {
            List<AutocompleteDataOut> autoCompleteCodesDataOut = CodesDataOut
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id.ToString(), 
                    text = x.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)
                })
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = autoCompleteCodesDataOut
            };

            return result;
        }

        public void Delete(int id)
        {
            personnelTeamDAL.Delete(id);
        }

        public int CountTeamsPerOrganization(int organizationId)
        {
            return personnelTeamDAL.GetAllEntriesCount(new PersonnelTeamFilter() { OrganizationId = organizationId, Page = 1, PageSize = 5 });
        }

        public bool IsPersonnelUnique(int userId, int personnelTeamId)
        {
            bool result = true;
            if(personnelTeamId != 0 && userId != 0)
            {
                PersonnelTeamDataOut personnelTeamDataOut = GetById(personnelTeamId);
                foreach (PersonnelTeamRelationDataOut personnel in personnelTeamDataOut.PersonnelTeamRelations)
                {
                    if (personnel.UserId == userId)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        public bool IsLeaderUnique(int roleCD, int? leaderRoleCD, int personnelTeamId)
        {
            bool result = true;
            
            if(personnelTeamId != 0 && leaderRoleCD != null && leaderRoleCD == roleCD)
            {
                PersonnelTeamDataOut personnelTeamDataOut = GetById(personnelTeamId);
                foreach (PersonnelTeamRelationDataOut personnel in personnelTeamDataOut.PersonnelTeamRelations)
                {
                    if (personnel.RelationType.Id == leaderRoleCD)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
