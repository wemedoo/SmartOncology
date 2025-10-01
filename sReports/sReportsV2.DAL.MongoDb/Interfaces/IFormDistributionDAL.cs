using sReportsV2.Common.Entities;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormDistributionDAL : IReplaceThesaurusDAL
    {
        IQueryable<FormDistribution> GetAll(EntityFilter filterData);
        int GetAllCount();
        FormDistribution GetById(string id);
        FormDistribution GetByThesaurusIdAndVersion(int id, string versionId);
        FormDistribution GetByThesaurusId(int id);
        FormDistribution InsertOrUpdate(FormDistribution formDistribution);
        List<FormDistribution> GetAll();
        List<FormDistribution> GetAllVersionAndThesaurus();
        FormFieldDistribution GetFormFieldDistribution(string formDistributionId, string fieldId);
    }
}
