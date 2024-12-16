using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.Encounter.DataOut;
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
        private readonly IMapper Mapper;

        public EncounterBLL(IEncounterDAL encounterDAL, IFormInstanceDAL formInstanceDAL, IFormDAL formDAL, IPersonnelDAL personnelDAL, IMapper mapper)
        {
            this.encounterDAL = encounterDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.formDAL = formDAL;
            this.personnelDAL = personnelDAL;
            Mapper = mapper;
        }

        public async Task DeleteAsync(int id)
        {
            await encounterDAL.DeleteAsync(id);
            await formInstanceDAL.DeleteByEncounterIdAsync(id);
        }

        public async Task<PaginationDataOut<EncounterViewDataOut, DataIn>> GetAllFiltered(EncounterFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            EncounterFilter filter = Mapper.Map<EncounterFilter>(dataIn);

            var dataTask = await encounterDAL.GetAllAsync(filter);
            var countTask = await  encounterDAL.GetAllEntriesCountAsync(filter);

            PaginationDataOut<EncounterViewDataOut, DataIn> result = new PaginationDataOut<EncounterViewDataOut, DataIn>()
            {
                Count = countTask,
                Data = Mapper.Map<List<EncounterViewDataOut>>(dataTask),
                DataIn = dataIn
            };

            return result;
        }

        public async Task<EncounterDetailsPatientTreeDataOut> GetEncounterAndFormInstancesAndSuggestedForms(int encounterId, UserCookieData userCookieData)
        {
            Encounter encounter = await encounterDAL.GetByIdAsync(encounterId).ConfigureAwait(false);
            if (encounter == null)
            {
                throw new ArgumentNullException(nameof(encounterId), "Please choose episode of care!");
            }

            var encounterDataOut = Mapper.Map<EncounterDataOut>(encounter);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceDAL.GetAllByEncounterAsync(encounterId, userCookieData.ActiveOrganization);
            Task<List<Form>> suggestedFormsTask = this.formDAL.GetByFormIdsListAsync(userCookieData.SuggestedForms);
            await Task.WhenAll(formInstancesTask, suggestedFormsTask).ConfigureAwait(false);

            EncounterDetailsPatientTreeDataOut result = new EncounterDetailsPatientTreeDataOut()
            {
                Encounter = encounterDataOut,
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(formInstancesTask.Result),
                Forms = Mapper.Map<List<FormDataOut>>(suggestedFormsTask.Result)
            };

            return result;
        }

        public List<EncounterDataOut> GetAllByEocId(int episodeOfCareId)
        {
            return Mapper.Map<List<EncounterDataOut>>(encounterDAL.GetAllByEocId(episodeOfCareId));
        }

        public async Task<int> InsertOrUpdateAsync(EncounterDataIn encounterData)
        {
            encounterData = Ensure.IsNotNull(encounterData, nameof(encounterData));
            Encounter encounter = Mapper.Map<Encounter>(encounterData);

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
            Encounter encounter = Mapper.Map<Encounter>(encounterData);

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

        public async Task<List<FormDataOut>> ListForms(string condition, UserCookieData userCookieData)
        {
            List<Form> result = await this.formDAL.GetAllByOrganizationAndLanguageAndNameAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, condition).ConfigureAwait(false);

            return Mapper.Map<List<FormDataOut>>(result.OrderBy(d => userCookieData.SuggestedForms.IndexOf(d.Id)).ToList());
        }

        public async Task<EncounterDetailsPatientTreeDataOut> ListReferralsAndForms(int encounterId, int episodeOfCareId, UserCookieData userCookieData)
        {
            Task<List<Form>> formsTask = this.formDAL.GetAllByOrganizationAndLanguageAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceDAL.GetAllByEpisodeOfCareIdAsync(episodeOfCareId, userCookieData.ActiveOrganization);

            await Task.WhenAll(formsTask, formInstancesTask).ConfigureAwait(false);
            EncounterDetailsPatientTreeDataOut result = new EncounterDetailsPatientTreeDataOut()
            {
                Encounter = new EncounterDataOut()
                {
                    Id = encounterId,
                    EpisodeOfCareId = episodeOfCareId
                },
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(formInstancesTask.Result),
                Forms = Mapper.Map<List<FormDataOut>>(formsTask.Result.OrderByDescending(d => userCookieData.SuggestedForms.IndexOf(d.Id)).ToList())
            };

            return result;
        }

        public async Task<List<Form>> GetSuggestedForms(List<string> suggestedFormsIds)
        {
            return await formDAL.GetByFormIdsListAsync(suggestedFormsIds);
        }

        public async Task<EncounterDataOut> GetByEncounterIdAsync(int encounterId, int organizationid, bool onlyEncounter)
        {
            EncounterDataOut encounter = Mapper.Map<EncounterDataOut>(await encounterDAL.GetByIdAsync(encounterId));
            if (onlyEncounter || encounter == null) return encounter;

            List<FormInstance> formInstances = await formInstanceDAL.GetAllByEncounterAsync(encounterId, organizationid);
            Task<IDictionary<int, string>> usersTask = personnelDAL.GetUsersDictionaryAsync(formInstances.Select(x => x.UserId).Distinct());

            encounter.FormInstances = Mapper.Map<List<PatientFormInstanceDataOut>>(formInstances);
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
            var encountersDataOut = Mapper.Map<List<EncounterDataOut>>(await encounterTask.ConfigureAwait(false));
            List<EncounterDataOut> filtered = encountersDataOut.Where(enc => enc.TypeId == encounterTypeId).OrderBy(x => x.Period.StartDate).ToList();

            return filtered;
        }
    }
}
