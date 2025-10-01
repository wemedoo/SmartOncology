using AutoMapper;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.DTOs.Field.DataOut.Custom;
using sReportsV2.DTOs.FormInstance;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using System.Text;

namespace sReportsV2.Controllers
{
    public class CustomFieldFilterController : BaseController
    {
        private readonly ICustomFieldFilterBLL customFieldFilterBLL;
        private readonly IMapper mapper;

        public CustomFieldFilterController(ICustomFieldFilterBLL customFieldFilterBLL, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IAsyncRunner asyncRunner,
            ICacheRefreshService cacheRefreshService) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.customFieldFilterBLL = customFieldFilterBLL;
            this.mapper = mapper;
        }

        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public ActionResult SaveCustomFieldFilter(FormInstanceFilterDataIn formInstanceFilterDataIn)
        {
            // README : Input Code to test this End Point.
            // Note: This document is specific to my DB. Please fill data correctly when testing.

            //FormInstanceFilterDataIn dataIn = new FormInstanceFilterDataIn();

            //CustomFieldFilterDataIn f1 = new CustomFieldFilterDataIn() { FieldType = FieldTypes.Datetime, FieldThesaurusId = 9566, Value = "02/12/2022,2022-12-02T00:30,00:30", FilterOperator = FieldOperators.Equal, FieldLabel = "Field0" };
            //CustomFieldFilterDataIn f2 = new CustomFieldFilterDataIn() { FieldType = FieldTypes.Datetime, FieldThesaurusId = 9566, Value = "01/12/2022,2022-12-01T00:00,00:00", FilterOperator = FieldOperators.GreaterThanOrEqual, FieldLabel = "Field1" };
            //List<CustomFieldFilterDataIn> fieldFiltersDataIn = new List<CustomFieldFilterDataIn>() { f1, f2 };

            //dataIn.FormId = "63282e2e4d38e3f3841e1f3b";
            //dataIn.FieldFiltersOverallOperator = LogicalOperators.OR;
            //dataIn.CustomFieldFiltersDataIn = fieldFiltersDataIn;
            //formInstanceFilterDataIn = dataIn;

            List<CustomFieldFilterData> customFieldFiltersData = mapper.Map<List<CustomFieldFilterData>>(formInstanceFilterDataIn.CustomFieldFiltersDataIn);

            CustomFieldFilterGroup dataToSave = new CustomFieldFilterGroup() { 
                CustomFieldFiltersData = customFieldFiltersData, 
                FormDefinitonId = formInstanceFilterDataIn.FormId, 
                OverallOperator = formInstanceFilterDataIn.FieldFiltersOverallOperator 
            };
            string writtenFilterId = customFieldFilterBLL.InsertOrUpdateCustomFieldFilter(dataToSave);

            return Content(writtenFilterId); // temporary for debug
        }

        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Engine)]
        public ActionResult LoadCustomFieldFiltersByFormId(string formDefinitionId)
        {
            List<CustomFieldFilterGroup> customFieldFilterGroups = customFieldFilterBLL.GetCustomFieldFiltersByFormId(formDefinitionId);

            List<CustomFieldFilterDataOut> dataOut = mapper.Map<List<CustomFieldFilterDataOut>>(customFieldFilterGroups);

            StringBuilder textBuilder = new StringBuilder();
            foreach (var d in dataOut)
                textBuilder.Append(d.RenderCustomFilterText());

            return Content(textBuilder.ToString());  // temporary for debug
        }
    }
}