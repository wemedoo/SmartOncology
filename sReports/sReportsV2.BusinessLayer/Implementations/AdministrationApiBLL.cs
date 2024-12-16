using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.ApiRequest;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.AdministrationApi.DataIn;
using sReportsV2.DTOs.AdministrationApi.DataOut;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class AdministrationApiBLL : IAdministrationApiBLL
    {
        private readonly IAdministrationApiDAL administrationApiDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IMapper mapper;

        public AdministrationApiBLL(IAdministrationApiDAL administrationApiDAL, IPatientDAL patientDAL, IFormInstanceDAL formInstanceDAL, IMapper mapper)
        {
            this.administrationApiDAL = administrationApiDAL;
            this.patientDAL = patientDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.mapper = mapper;
        }

        public async Task<PaginationDataOut<AdministrationApiDataOut, DataIn>> ReloadTable(AdministrationApiFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            AdministrationApiFilter filterData = mapper.Map<AdministrationApiFilter>(dataIn);
            await AddAdditionalFilterParams(filterData);
            PaginationDataOut<AdministrationApiDataOut, DataIn> result = new PaginationDataOut<AdministrationApiDataOut, DataIn>()
            {
                Count = (int) await administrationApiDAL.GetAllFilteredCount(filterData),
                Data = mapper.Map<List<AdministrationApiDataOut>>(await administrationApiDAL.GetAll(filterData)),
                DataIn = dataIn
            };

            return result;
        }

        public async Task<AdministrationApiDataOut> ViewLog(int apiRequestLogId)
        {
            ApiRequestLog entity = await administrationApiDAL.ViewLog(apiRequestLogId);
            return mapper.Map<AdministrationApiDataOut>(entity);
        }

        private async Task AddAdditionalFilterParams(AdministrationApiFilter filterData)
        {
            if (!string.IsNullOrEmpty(filterData.ScreeningNumber))
            {
                int? oomniaExternalCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PatientIdentifierType, ResourceTypes.OomniaScreeningNumber);
                Patient patient = await patientDAL.GetByIdentifierAsync(new Domain.Sql.Entities.Patient.PatientIdentifier
                {
                    IdentifierValue = filterData.ScreeningNumber,
                    IdentifierTypeCD = oomniaExternalCodeId
                });
                if (patient != null)
                {
                    List<FormInstance> formInstances = formInstanceDAL.GetByPatient(patient.PatientId);
                    if (formInstances.Count == 0 || formInstances.Count > 1)
                    {
                        throw new UserAdministrationException();
                    }
                    else
                    {
                        Guid? oomniaDocumentInstanceExternalId = formInstances.First().OomniaDocumentInstanceExternalId;
                        if (oomniaDocumentInstanceExternalId.HasValue)
                        {
                            filterData.RequestContains = oomniaDocumentInstanceExternalId.ToString();
                        }
                    }
                }
            }
        }
    }
}
