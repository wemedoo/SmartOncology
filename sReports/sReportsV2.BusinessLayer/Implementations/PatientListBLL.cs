using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.PatientList;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.PatientList;
using sReportsV2.DTOs.DTOs.PatientList.DataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PatientListBLL : IPatientListBLL
    {
        private readonly IPatientListDAL patientListDAL;
        private readonly IPersonnelDAL personnelDAL;
        private readonly IMapper mapper;
        private readonly ICodeDAL codeDAL;

        public PatientListBLL(IPatientListDAL patientListDAL, IPersonnelDAL personnelDAL, IMapper mapper, ICodeDAL codeDAL)
        {
            this.patientListDAL = patientListDAL;
            this.personnelDAL = personnelDAL;
            this.mapper = mapper;
            this.codeDAL = codeDAL;
        }

        public async Task<PaginationDataOut<PatientListDTO, PatientListFilterDataIn>> GetAll(PatientListFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PatientListFilter patientListFilter = mapper.Map<PatientListFilter>(dataIn);

            PaginationData<PatientList> patientListPagination = await patientListDAL.GetAllByFilter(patientListFilter).ConfigureAwait(false);
  
            return new PaginationDataOut<PatientListDTO, PatientListFilterDataIn>
            {
                Data = mapper.Map<List<PatientListDTO>>(patientListPagination.Data),
                Count = patientListPagination.Count,
                DataIn = dataIn
            };
        }

        public async Task<PatientListDTO> GetById(int? id)
        {
            if (id != null && id > 0)
            {
                return mapper.Map<PatientListDTO>(await patientListDAL.GetByIdAsync((int)id).ConfigureAwait(false));
            }
            return null;
        }

        public async Task<PatientListDTO> Create(PatientListDTO patientListDTO)
        {
            patientListDTO = Ensure.IsNotNull(patientListDTO, nameof(patientListDTO));
            PatientList patientList = mapper.Map<PatientList>(patientListDTO);
            patientList.SetActiveFromAndToDatetime();
            AddCreatorToPersonnelRelations(patientList);
            return mapper.Map<PatientListDTO>(
                await patientListDAL.Create(patientList).ConfigureAwait(false));
        }

        public async Task<PatientListDTO> Edit(PatientListDTO patientListDTO)
        {
            patientListDTO = Ensure.IsNotNull(patientListDTO, nameof(patientListDTO));
            
            return mapper.Map<PatientListDTO>( 
                await patientListDAL.Edit(mapper.Map<PatientList>(patientListDTO)).ConfigureAwait(false));
        }

        public async Task Delete(int? id)
        {
            if(id != null && id > 0)
                await patientListDAL.Delete((int)id);
        }

        public async Task<PatientListPersonnelRelationDTO> AddPersonnelRelation(PatientListPersonnelRelationDTO patientListPersonnelRelationDTO)
        {
            patientListPersonnelRelationDTO = Ensure.IsNotNull(patientListPersonnelRelationDTO, nameof(patientListPersonnelRelationDTO));

            if(patientListPersonnelRelationDTO.PersonnelId > 0 && patientListPersonnelRelationDTO.PatientListId > 0)
            {
                return mapper.Map<PatientListPersonnelRelationDTO>(
                    await patientListDAL.AddPersonnelRelation(mapper.Map<PatientListPersonnelRelation>(patientListPersonnelRelationDTO)).ConfigureAwait(false));
            }
            return patientListPersonnelRelationDTO;
        }

        public async Task<List<PatientListPersonnelRelationDTO>> AddPersonnelRelations(List<PatientListPersonnelRelationDTO> patientListPersonnelRelationDTOs)
        {
            if(patientListPersonnelRelationDTOs != null && patientListPersonnelRelationDTOs.Count > 0 && patientListPersonnelRelationDTOs.TrueForAll(x => x.PatientListId > 0 && x.PersonnelId > 0))
            {
                return mapper.Map<List<PatientListPersonnelRelationDTO>>(
                    await patientListDAL.AddPersonnelRelations(mapper.Map<List<PatientListPersonnelRelation>>(patientListPersonnelRelationDTOs)).ConfigureAwait(false));
            }
            return patientListPersonnelRelationDTOs;
        }

        public async Task RemovePersonnelRelation(int patientListId, int personnelId)
        {
            if(patientListId > 0 && personnelId > 0)
            {
                await patientListDAL.RemovePersonnelRelation(patientListId, personnelId);
            }
        }

        public async Task<AutocompleteResultDataOut> GetAutoCompleteName(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            EntityFilter filter = new EntityFilter() { Page = dataIn.Page, PageSize = FilterConstants.DefaultPageSize };
            PaginationData<AutoCompleteData> patientListsAndCount = await patientListDAL.GetTrialAutoCompleteNameAndCount(dataIn.Term, filter);

            List<AutocompleteDataOut> autocompleteDataDataOuts = patientListsAndCount.Data
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id,
                    text = x.Text,
                })
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                results = autocompleteDataDataOuts,
                pagination = new AutocompletePaginatioDataOut() { more = dataIn.ShouldLoadMore(patientListsAndCount.Count) }
            };

            return result;
        }

        public async Task<PaginationDataOut<UserDataOut, DataIn>> ReloadPersonnelTable(PatientListFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PatientListFilter filter = mapper.Map<PatientListFilter>(dataIn);
            int? archivedUserStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.UserState, CodeAttributeNames.Archived);

            PaginationData<Personnel> personnelPagination = await personnelDAL.GetAllPatientListPersonnelsAndCount(filter, archivedUserStateCD);

            return new PaginationDataOut<UserDataOut, DataIn>
            {
                Data = mapper.Map<List<UserDataOut>>(personnelPagination.Data),
                Count = personnelPagination.Count,
                DataIn = dataIn
            };
        }

        public async Task AddPatientRelations(PatientListPatientRelationDTO patientListPatientRelationDTO)
        {
            patientListPatientRelationDTO = Ensure.IsNotNull(patientListPatientRelationDTO, nameof(patientListPatientRelationDTO));
            PatientListPatientRelation patientListPatientRelation = mapper.Map<PatientListPatientRelation>(patientListPatientRelationDTO);
            if (patientListPatientRelation.PatientListId > 0 && patientListPatientRelation.PatientId > 0)
            {
                await patientListDAL.AddPatientRelation(patientListPatientRelation);
            }
        }

        public async Task RemovePatientRelation(PatientListPatientRelationDTO patientListPatientRelationDTO)
        {
            patientListPatientRelationDTO = Ensure.IsNotNull(patientListPatientRelationDTO, nameof(patientListPatientRelationDTO));
            PatientListPatientRelation patientListPatientRelation = mapper.Map<PatientListPatientRelation>(patientListPatientRelationDTO);
            if (patientListPatientRelation.PatientListId > 0 && patientListPatientRelation.PatientId > 0)
            {
                await patientListDAL.RemovePatientRelation(patientListPatientRelation);
            }
        }

        public async Task<Dictionary<int, IEnumerable<PatientListDTO>>> GetListsAvailableForPatients(List<PatientDataOut> patients, int activePersonnelId)
        {
            var patientIds = patients.Select(x => x.Id).Distinct();
            Dictionary<int, IEnumerable<int>> patientsAndLists = await patientListDAL.GetListsContainingPatients(patientIds).ConfigureAwait(false);
            List<PatientListDTO> listWithSelectedPatients = mapper.Map<List<PatientListDTO>>(
                (await patientListDAL.GetAllByFilter(new PatientListFilter() { PersonnelId = activePersonnelId, Page = 1, PageSize = 30, ListWithSelectedPatients = true}).ConfigureAwait(false))
                .Data);

            Dictionary<int, IEnumerable<PatientListDTO>> result = new Dictionary<int, IEnumerable<PatientListDTO>>();

            foreach(int patientId in patientIds)
            {
                if(patientsAndLists.ContainsKey(patientId))
                    result.Add(patientId, listWithSelectedPatients.Where(x => !patientsAndLists[patientId].Contains(x.PatientListId)));
                else
                    result.Add(patientId, listWithSelectedPatients);
            }
            return result;
        }

        private void AddCreatorToPersonnelRelations(PatientList patientList)
        {
            patientList.PatientListPersonnelRelations.Add( new PatientListPersonnelRelation() { 
                PersonnelId = patientList.CreatedById.Value, 
                CreatedById = patientList.CreatedById.Value });
        }
    }
}
