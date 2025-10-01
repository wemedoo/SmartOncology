using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class EncounterBLL : IEncounterBLL
    {
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IFormDAL formDAL;
        private readonly IPersonnelDAL personnelDAL;
        private readonly IMapper mapper;

        public EncounterBLL(IEncounterDAL encounterDAL, IFormInstanceDAL formInstanceDAL, IFormDAL formDAL, IPersonnelDAL personnelDAL, IMapper mapper)
        {
            this.encounterDAL = encounterDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.formDAL = formDAL;
            this.personnelDAL = personnelDAL;
            this.mapper = mapper;
        }

        public async Task DeleteAsync(int id)
        {
            await encounterDAL.DeleteAsync(id);
            await formInstanceDAL.DeleteByEncounterIdAsync(id);
        }

        public async Task<PaginationDataOut<EncounterViewDataOut, DataIn>> GetAllFiltered(EncounterFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            EncounterFilter filter = mapper.Map<EncounterFilter>(dataIn);

            var dataTask = await encounterDAL.GetAllAsync(filter);
            var countTask = await  encounterDAL.GetAllEntriesCountAsync(filter);

            PaginationDataOut<EncounterViewDataOut, DataIn> result = new PaginationDataOut<EncounterViewDataOut, DataIn>()
            {
                Count = countTask,
                Data = mapper.Map<List<EncounterViewDataOut>>(dataTask),
                DataIn = dataIn
            };

            return result;
        }

        public List<EncounterDataOut> GetAllByEocId(int episodeOfCareId)
        {
            return mapper.Map<List<EncounterDataOut>>(encounterDAL.GetAllByEocId(episodeOfCareId));
        }

        public async Task<int> InsertOrUpdateAsync(EncounterDataIn encounterData)
        {
            encounterData = Ensure.IsNotNull(encounterData, nameof(encounterData));
            Encounter encounter = mapper.Map<Encounter>(encounterData);

            Encounter encounterDB = await encounterDAL.GetByIdAsync(encounter.EncounterId).ConfigureAwait(false);

            if (encounterDB == null)
            {
                encounterDB = encounter;
            }
            else
            {
                encounterDB.Copy(encounter);
            }

            return await encounterDAL.InsertOrUpdateAsync(encounterDB).ConfigureAwait(false);
        }

        public int InsertOrUpdate(EncounterDataIn encounterData)
        {
            encounterData = Ensure.IsNotNull(encounterData, nameof(encounterData));
            Encounter encounter = mapper.Map<Encounter>(encounterData);

            Encounter encounterDB = encounterDAL.GetById(encounter.EncounterId);

            if (encounterDB == null)
            {
                encounterDB = encounter;
            }
            else
            {
                encounterDB.Copy(encounter);
            }

            return encounterDAL.InsertOrUpdate(encounterDB);
        }

        public async Task<List<AutocompleteOptionDataOut>> ListForms(string searchName, UserCookieData userCookieData)
        {
            List<Form> result = await this.formDAL.GetAllByOrganizationAndLanguageAndNameAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, searchName).ConfigureAwait(false);

            return result.OrderBy(d => userCookieData.SuggestedForms.IndexOf(d.Id)).Select(x => new AutocompleteOptionDataOut(x, userCookieData)).ToList();
        }

        public async Task<EncounterDetailsPatientTreeDataOut> ListReferralsAndForms(int encounterId, int episodeOfCareId, UserCookieData userCookieData)
        {
            Task<List<Form>> formsTask = this.formDAL.GetAllByOrganizationAndLanguageAndNameAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceDAL.GetAllByEpisodeOfCareIdAsync(episodeOfCareId, userCookieData.ActiveOrganization);

            await Task.WhenAll(formsTask, formInstancesTask).ConfigureAwait(false);
            EncounterDetailsPatientTreeDataOut result = new EncounterDetailsPatientTreeDataOut()
            {
                Encounter = new EncounterDataOut()
                {
                    Id = encounterId,
                    EpisodeOfCareId = episodeOfCareId
                },
                FormInstances = mapper.Map<List<FormInstanceMetadataDataOut>>(formInstancesTask.Result),
                Forms = formsTask.Result.OrderByDescending(d => userCookieData.SuggestedForms.IndexOf(d.Id)).Select(x => new AutocompleteOptionDataOut(x, userCookieData)).ToList()
            };

            return result;
        }

        public async Task<List<Form>> GetSuggestedForms(List<string> suggestedFormsIds)
        {
            return await formDAL.GetByFormIdsListAsync(suggestedFormsIds);
        }

        public async Task<EncounterDataOut> GetByEncounterIdAsync(int encounterId, int organizationid, bool onlyEncounter)
        {
            EncounterDataOut encounter = mapper.Map<EncounterDataOut>(await encounterDAL.GetByIdAsync(encounterId));
            if (onlyEncounter || encounter == null) return encounter;

            List<FormInstance> formInstances = await formInstanceDAL.GetAllByEncounterAsync(encounterId, organizationid);
            Task<IDictionary<int, string>> usersTask = personnelDAL.GetUsersDictionaryAsync(formInstances.Select(x => x.UserId).Distinct());

            encounter.FormInstances = mapper.Map<List<PatientFormInstanceDataOut>>(formInstances);
            IDictionary<int, string> users = await usersTask;

            foreach (var formInstanceDataOut in encounter.FormInstances)
                formInstanceDataOut.ShortNameInfo = users[formInstanceDataOut.UserId];

            return encounter;
        }

        public int GetEncounterTypeByEncounterId(int encounterId)
        {
            return encounterDAL.GetEncounterTypeByEncounterId(encounterId);
        }

        public async Task<List<EncounterDataOut>> GetEncountersByTypeAndEocIdAsync(int encounterTypeId, int episodeOfCareId)
        {
            var encounterTask = encounterDAL.GetByEOCIdAsync(episodeOfCareId);
            var encountersDataOut = mapper.Map<List<EncounterDataOut>>(await encounterTask.ConfigureAwait(false));
            List<EncounterDataOut> filtered = encountersDataOut.Where(enc => enc.TypeId == encounterTypeId).OrderBy(x => x.Period.StartDate).ToList();

            return filtered;
        }
    }
}
