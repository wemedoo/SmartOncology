using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PersonnelTeamRelationBLL : IPersonnelTeamRelationBLL
    {
        private readonly IPersonnelTeamRelationDAL personnelTeamRelationDAL;
        private readonly IMapper mapper;

        public PersonnelTeamRelationBLL(IPersonnelTeamRelationDAL personnelTeamRelationDAL, IMapper mapper)
        {
            this.personnelTeamRelationDAL = personnelTeamRelationDAL;
            this.mapper = mapper;
        }

        public PersonnelTeamRelationDataOut GetById(int id)
        {
            return mapper.Map<PersonnelTeamRelationDataOut>(personnelTeamRelationDAL.GetById(id));
        }

        public void InsertOrUpdate(PersonnelTeamRelationDataIn personnelTeamRelationDataIn)
        {
            PersonnelTeamRelation personnelTeamRelation = mapper.Map<PersonnelTeamRelation>(personnelTeamRelationDataIn);

            if (personnelTeamRelation.PersonnelTeamRelationId != 0)
            {
                PersonnelTeamRelation dbPersonnelTeamRelation = personnelTeamRelationDAL.GetById(personnelTeamRelation.PersonnelTeamRelationId);
                if (dbPersonnelTeamRelation == null) throw new KeyNotFoundException();

                dbPersonnelTeamRelation.CopyData(personnelTeamRelation);
                personnelTeamRelation = dbPersonnelTeamRelation;
            }
            personnelTeamRelationDAL.InsertOrUpdate(personnelTeamRelation);
        }

        public void InsertMany(List<PersonnelTeamRelationDataIn> personnelTeamRelationDataIns)
        {
            foreach (PersonnelTeamRelationDataIn personnelTeamRelation in personnelTeamRelationDataIns)
                InsertOrUpdate(personnelTeamRelation);
        }

        public void Delete(int personnelTeamRelationId)
        {
            personnelTeamRelationDAL.Delete(personnelTeamRelationId);
        }

        public PaginationDataOut<PersonnelTeamRelationDataOut, DataIn> GetAllFiltered(PersonnelTeamRelationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            PersonnelTeamRelationFilter filter = mapper.Map<PersonnelTeamRelationFilter>(dataIn);
            PaginationDataOut<PersonnelTeamRelationDataOut, DataIn> result = new PaginationDataOut<PersonnelTeamRelationDataOut, DataIn>()
            {
                Count = personnelTeamRelationDAL.GetAllEntriesCount(filter),
                Data = mapper.Map<List<PersonnelTeamRelationDataOut>>(personnelTeamRelationDAL.GetAll(filter)),
                DataIn = dataIn
            };
            return result;
        }

        public AutocompleteResultDataOut GetNameForAutocomplete(AutocompleteDataIn autocompleteDataIn, int personnelTeamId)
        {
            autocompleteDataIn = Ensure.IsNotNull(autocompleteDataIn, nameof(autocompleteDataIn));

            IQueryable<PersonnelTeamRelation> filtered = personnelTeamRelationDAL.FilterByName(autocompleteDataIn.Term)
                .Where(x => x.PersonnelTeamId == personnelTeamId)
                .OrderBy(x => x.Personnel.FirstName);

            List<AutocompleteDataOut> personnelTeamRelationDataOuts = filtered
                .AsEnumerable()
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.PersonnelId.ToString(),
                    text = x.Personnel.GetFirstAndLastName()
                }).ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = personnelTeamRelationDataOuts
            };

            return result;
        }

        public AutocompleteResultDataOut GetPersonnelTeamRelationshipTypeCodes(string activeLanguage, List<CodeDataOut> CodesDataOut)
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
    }
}
