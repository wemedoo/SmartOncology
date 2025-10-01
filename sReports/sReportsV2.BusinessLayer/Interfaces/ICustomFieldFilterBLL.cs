using sReportsV2.Domain.Entities.CustomFieldFilters;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICustomFieldFilterBLL
    {
        string InsertOrUpdateCustomFieldFilter(CustomFieldFilterGroup dataToSave);
        List<CustomFieldFilterGroup> GetCustomFieldFiltersByFormId(string formDefinitionId);
    }
}
