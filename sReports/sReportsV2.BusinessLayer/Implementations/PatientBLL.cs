using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.DTOs.TrialManagement;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.Initializer.CodeSets;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PatientBLL : IPatientBLL
    {
        private readonly IPatientDAL patientDAL;
        private readonly ICodeDAL codeDAL;
        private readonly IPatientListDAL patientListDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IConfiguration configuration;
        private readonly IMapper Mapper;
        private readonly ITrialManagementDAL trialManagementDAL;

        public PatientBLL(IPatientDAL patientDAL, ICodeDAL codeDAL, IPatientListDAL patientListDAL, IFormInstanceDAL formInstanceDAL, IConfiguration configuration, IMapper mapper, ITrialManagementDAL trialManagementDAL)
        {
            this.patientDAL = patientDAL;
            this.codeDAL = codeDAL;
            this.patientListDAL = patientListDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.configuration = configuration;
            Mapper = mapper;
            this.trialManagementDAL = trialManagementDAL;
        }

        #region CRUD
        public PatientDataOut GetById(int id, bool loadClinicalTrials)
        {
            Patient patient = patientDAL.GetById(id) ?? throw new ArgumentNullException(nameof(id), "Patient does not exist");
            PatientDataOut patientDataOut = Mapper.Map<PatientDataOut>(patient);
            if (loadClinicalTrials)
            {
                patientDataOut.ClinicalTrials = Mapper.Map<List<ClinicalTrialDataOut>>(trialManagementDAL.GetlClinicalTrialByIds(patient.PatientChemotherapyData?.GetClinicalTrialIds()));
            }

            return patientDataOut;
        }

        public async Task<PatientDataOut> GetByIdAsync(int id)
        {
            Patient patient = await patientDAL.GetByIdAsync(id).ConfigureAwait(false);
            if (patient == null) throw new ArgumentNullException(nameof(id), "Patient does not exist");

            return Mapper.Map<PatientDataOut>(patient);
        }

        public async Task<PatientDataOut> GetByIdentifierAsync(PatientIdentifier identifier)
        {
            Task<Patient> patient = patientDAL.GetByIdentifierAsync(identifier);
            if (patient == null) throw new ArgumentNullException(nameof(identifier), "Patient does not exist");

            return Mapper.Map<PatientDataOut>(await patient);
        }

        public PatientTableDataOut GetPreviewById(int id)
        {
            PatientTableDataOut patientTableDataOut = Mapper.Map<PatientTableDataOut>(patientDAL.GetById(id));
            return patientTableDataOut ?? throw new ArgumentNullException(nameof(id), "Patient does not exist");
        }

        public ResourceCreatedDTO InsertOrUpdate(PatientDataIn patientDataIn, UserData userData)
        {
            Patient patient = Mapper.Map<Patient>(patientDataIn);
            patient.OrganizationId = userData.ActiveOrganization.GetValueOrDefault();
            Patient patientDb = patientDAL.GetById(patient.PatientId);
            PatientIdentifierDataIn defaultIdentifier = null;

            if (patientDb == null)
            {
                patientDb = patient;
                if (!string.IsNullOrWhiteSpace(configuration["DefaultIdentifier"]))
                {
                    defaultIdentifier = new PatientIdentifierDataIn
                    {
                        IdentifierTypeCD = codeDAL.GetByPreferredTerm(configuration["DefaultIdentifier"], (int)CodeSetList.PatientIdentifierType)?.CodeId
                    };
                }
            }
            else
            {
                patientDb.Copy(patient, false);
            }
            patientDAL.InsertOrUpdate(patientDb, Mapper.Map<PatientIdentifier>(defaultIdentifier));

            return new ResourceCreatedDTO()
            {
                Id = patientDb.PatientId.ToString(),
                RowVersion = Convert.ToBase64String(patientDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PatientContactDataIn patientContactDataIn)
        {
            PatientContact patientContact = Mapper.Map<PatientContact>(patientContactDataIn);
            PatientContact patientContactDb = await patientDAL.GetById(new QueryEntityParam<PatientContact>(patientContact.PatientContactId)).ConfigureAwait(false);

            if (patientContactDb == null)
            {
                patientContactDb = patientContact;
            }
            else
            {
                patientContactDb.Copy(patientContact);
            }
            await patientDAL.InsertOrUpdate(patientContactDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = patientContactDb.PatientContactId.ToString(),
                RowVersion = Convert.ToBase64String(patientContactDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PatientIdentifierDataIn childDataIn)
        {
            PatientIdentifier patientIdentifier = Mapper.Map<PatientIdentifier>(childDataIn);
            PatientIdentifier patientIdentifierDb = await patientDAL.GetById(new QueryEntityParam<PatientIdentifier>(childDataIn.Id)).ConfigureAwait(false);

            if (patientIdentifierDb == null)
            {
                patientIdentifierDb = patientIdentifier;
            }
            else
            {
                patientIdentifierDb.Copy(patientIdentifier);
            }
            await patientDAL.InsertOrUpdate(patientIdentifierDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = patientIdentifierDb.PatientIdentifierId.ToString(),
                RowVersion = Convert.ToBase64String(patientIdentifierDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PatientAddressDataIn addressDataIn)
        {
            PatientAddress patientAddress = Mapper.Map<PatientAddress>(addressDataIn);
            PatientAddress patientAddressDb = await patientDAL.GetById(new QueryEntityParam<PatientAddress>(addressDataIn.Id)).ConfigureAwait(false);

            if (patientAddressDb == null)
            {
                patientAddressDb = patientAddress;
            }
            else
            {
                patientAddressDb.Copy(patientAddress);
            }
            await patientDAL.InsertOrUpdate(patientAddressDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = patientAddressDb.PatientAddressId.ToString(),
                RowVersion = Convert.ToBase64String(patientAddressDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PatientContactAddressDataIn patientContactAddressDataIn)
        {
            PatientContactAddress patientContactAddress = Mapper.Map<PatientContactAddress>(patientContactAddressDataIn);
            PatientContactAddress patientContactAddressDb = await patientDAL.GetById(new QueryEntityParam<PatientContactAddress>(patientContactAddressDataIn.Id)).ConfigureAwait(false);

            if (patientContactAddressDb == null)
            {
                patientContactAddressDb = patientContactAddress;
            }
            else
            {
                patientContactAddressDb.Copy(patientContactAddress);
            }
            await patientDAL.InsertOrUpdate(patientContactAddressDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = patientContactAddressDb.PatientContactAddressId.ToString(),
                RowVersion = Convert.ToBase64String(patientContactAddressDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PatientTelecomDataIn telecomDataIn)
        {
            PatientTelecom patientTelecom = Mapper.Map<PatientTelecom>(telecomDataIn);
            PatientTelecom patientTelecomDb = await patientDAL.GetById(new QueryEntityParam<PatientTelecom>(telecomDataIn.Id)).ConfigureAwait(false);

            if (patientTelecomDb == null)
            {
                patientTelecomDb = patientTelecom;
            }
            else
            {
                patientTelecomDb.Copy(patientTelecom);
            }
            await patientDAL.InsertOrUpdate(patientTelecomDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = patientTelecomDb.PatientTelecomId.ToString(),
                RowVersion = Convert.ToBase64String(patientTelecomDb.RowVersion)
            };
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(PatientContactTelecomDataIn telecomDataIn)
        {
            PatientContactTelecom patientContactTelecom = Mapper.Map<PatientContactTelecom>(telecomDataIn);
            PatientContactTelecom patientContactTelecomDb = await patientDAL.GetById(new QueryEntityParam<PatientContactTelecom>(telecomDataIn.Id)).ConfigureAwait(false);

            if (patientContactTelecomDb == null)
            {
                patientContactTelecomDb = patientContactTelecom;
            }
            else
            {
                patientContactTelecomDb.Copy(patientContactTelecom);
            }
            await patientDAL.InsertOrUpdate(patientContactTelecomDb).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = patientContactTelecomDb.PatientContactTelecomId.ToString(),
                RowVersion = Convert.ToBase64String(patientContactTelecomDb.RowVersion)
            };
        }

        public async Task Delete(PatientDataIn patientDataIn)
        {
            try
            {
                Patient patient = Mapper.Map<Patient>(patientDataIn);
                await patientDAL.Delete(patient).ConfigureAwait(false);
                await formInstanceDAL.DeleteByPatientIdAsync(patientDataIn.Id).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PatientContactDataIn childDataIn)
        {
            try
            {
                PatientContact patientContact = Mapper.Map<PatientContact>(childDataIn);
                await patientDAL.Delete(patientContact).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PatientIdentifierDataIn childDataIn)
        {
            try
            {
                PatientIdentifier patientIdentifier = Mapper.Map<PatientIdentifier>(childDataIn);
                await patientDAL.Delete(patientIdentifier).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PatientAddressDataIn childDataIn)
        {
            try
            {
                PatientAddress patientAddress = Mapper.Map<PatientAddress>(childDataIn);
                await patientDAL.Delete(patientAddress).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PatientContactAddressDataIn childDataIn)
        {
            try
            {
                PatientContactAddress patientContactAddress = Mapper.Map<PatientContactAddress>(childDataIn);
                await patientDAL.Delete(patientContactAddress).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PatientTelecomDataIn childDataIn)
        {
            try
            {
                PatientTelecom patientTelecom = Mapper.Map<PatientTelecom>(childDataIn);
                await patientDAL.Delete(patientTelecom).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public async Task Delete(PatientContactTelecomDataIn childDataIn)
        {
            try
            {
                PatientContactTelecom patientTelecom = Mapper.Map<PatientContactTelecom>(childDataIn);
                await patientDAL.Delete(patientTelecom).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }
        #endregion /CRUD

        public bool ExistEntity(PatientIdentifierDataIn identifierDataIn)
        {
            PatientIdentifier identifier = Mapper.Map<PatientIdentifier>(identifierDataIn);
            return patientDAL.ExistsPatientByIdentifier(identifier);
        }


        public async Task<PaginationDataOut<T, PatientFilterDataIn>> GetAllFilteredAsync<T>(PatientFilterDataIn dataIn)
        {
            PatientFilter filter = Mapper.Map<PatientFilter>(dataIn);
            PopulateGenders(filter);
            await GetPatientListFilterAsync(dataIn, filter).ConfigureAwait(false);

            PaginationData<Patient> patientPagination = await patientDAL.GetAllAndCount(filter).ConfigureAwait(false);
            PaginationDataOut<T, PatientFilterDataIn> result = new PaginationDataOut<T, PatientFilterDataIn>()
            {
                Count = patientPagination.Count,
                Data = Mapper.Map<List<T>>(patientPagination.Data),
                DataIn = dataIn
            };
            return result;
        }

        public List<PatientTableDataOut> GetPatientsByFirstAndLastName(PatientByNameFilterDataIn patientByNameFilterDataIn)
        {
            PatientByNameSearchFilter patientSearchFilter = Mapper.Map<PatientByNameSearchFilter>(patientByNameFilterDataIn);
            return Mapper.Map<List<PatientTableDataOut>>(patientDAL.GetPatientsFilteredByFirstAndLastName(patientSearchFilter));
        }

        public AutocompleteResultDataOut GetAutocompletePatientData(AutocompleteDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = patientDAL.GetAll(new PatientFilter { OrganizationId = userCookieData.ActiveOrganization, Given = dataIn.Term, SimpleNameSearch = true, ApplyOrderByAndPagination = false });
            var enumDataOuts = filtered
                .OrderBy(x => x.NameGiven).Skip(dataIn.Page * 15).Take(15)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.PatientId.ToString(),
                    text = e.NameGiven + " " + e.NameFamily
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList()
                ;

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count / 15.00) > dataIn.Page,
                },
                results = enumDataOuts
            };

            return result;
        }

        public async Task<ContactDTO> GetContactById(int id)
        {
            PatientContact patientContact = await patientDAL.GetById(new QueryEntityParam<PatientContact>(id)).ConfigureAwait(false);

            return Mapper.Map<ContactDTO>(patientContact);
        }

        private void PopulateGenders(PatientFilter patientFilter)
        {
            List<CodeDataOut> genderCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender);
            string activeLanguage = patientFilter.ActiveLanguage;
            foreach (CodeDataOut genderCode in genderCodes)
            {
                string genderName = genderCode.Thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                if (!string.IsNullOrEmpty(genderName) && !patientFilter.Genders.ContainsKey(genderName))
                {
                    patientFilter.Genders.Add(genderName, new Tuple<int, string>(genderCode.Id, genderCode.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)));
                }
            }
        }

        private async Task GetPatientListFilterAsync(PatientFilterDataIn dataIn, PatientFilter filter)
        {
            if (dataIn.PatientListId != null && dataIn.PatientListId > 0)
            {
                filter.PatientList = await patientListDAL.GetByIdAsync(dataIn.PatientListId.Value).ConfigureAwait(false);
                filter.AttendingDoctorCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.RelationType, CodeSetValues.RelationType.ElementAtOrDefault(1));
            }
        }
    }
}
