using Microsoft.AspNetCore.Http;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataIn;
using System.Collections.Generic;

namespace sReportsV2.Common.Extensions
{
    public static class FormInstanceCacheExtensions
    {
        public static void SaveToCache(this ISession session, FormInstance formInstance)
        {
            string cacheKey = $"FormInstanceCache_{formInstance.Id}";
            session.SetObjectAsJson(cacheKey, GenerateFormInstanceCachedData(formInstance));
        }

        public static FormInstanceDataIn MergeFromCache(
            this ISession session,
            FormInstanceDataIn incoming)
        {
            string cacheKey = $"FormInstanceCache_{incoming.FormInstanceId}";
            FormInstanceDataIn cachedDto = session.GetObject<FormInstanceDataIn>(cacheKey);

            if (cachedDto == null)
                return incoming;

            cachedDto.UpdateCachedData(incoming);

            // save back updated cache
            session.SetObjectAsJson(cacheKey, cachedDto);

            return cachedDto;
        }

        private static FormInstanceDataIn GenerateFormInstanceCachedData(FormInstance formInstance)
        {
            FormInstanceDataIn formInstanceDataIn = new FormInstanceDataIn
            {
                FormInstanceId = formInstance.Id,
                Notes = formInstance.Notes,
                FormState = formInstance.FormState.ToString(),
                Date = formInstance.Date.GetDateTimeDisplay(DateTimeConstants.DateFormat, excludeTimePart: true),
                FieldInstances = new List<FieldInstanceDTO>()
            };
            foreach (FieldInstance fieldInstance in formInstance.FieldInstances) 
            {
                foreach (FieldInstanceValue fieldInstanceRepetition in fieldInstance.FieldInstanceValues)
                {
                    formInstanceDataIn.FieldInstances.Add(new FieldInstanceDTO(fieldInstance, fieldInstanceRepetition));
                }
            }
            return formInstanceDataIn;
        }
    }
}
