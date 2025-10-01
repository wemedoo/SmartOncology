using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Services.Interfaces;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class CustomFieldFilterBLL : ICustomFieldFilterBLL
    {
        private readonly ICustomFieldFilterDAL customFieldFilterDAL;

        public CustomFieldFilterBLL(ICustomFieldFilterDAL customFieldFilterDAL)
        {
            this.customFieldFilterDAL = customFieldFilterDAL;
        }

        public List<CustomFieldFilterGroup> GetCustomFieldFiltersByFormId(string formDefinitionId)
        {
            return customFieldFilterDAL.GetCustomFieldFiltersByFormId(formDefinitionId);
        }

        public string InsertOrUpdateCustomFieldFilter(CustomFieldFilterGroup dataToSave)
        {
            return customFieldFilterDAL.InsertOrUpdateCustomFieldFilter(dataToSave);
        }
    }
}
