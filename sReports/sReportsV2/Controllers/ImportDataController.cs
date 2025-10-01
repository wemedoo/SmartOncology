using sReportsV2.UMLS.Classes;
using System;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Initializer.OrganizationCSV;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Helpers;
using System.IO;

namespace sReportsV2.Controllers
{
    public class ImportDataController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration configuration;

        public ImportDataController(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public ActionResult InsertOrganizations()
        {
            ImportOrganization importer = new ImportOrganization(_serviceProvider.GetService<IOrganizationDAL>());
            int? countryCD = _serviceProvider.GetService<ICodeDAL>().GetByCodeSetIdAndPreferredTerm((int)CodeSetList.Country, ResourceTypes.CompanyCountry);
            importer.InsertOrganization(Path.Combine(DirectoryHelper.AppDataFolder, "SwissHospitals.csv"), countryCD);
            return Ok();
        }

        public ActionResult InsertCodingSystems()
        {
            ICodeSystemDAL codeSystemDAL = _serviceProvider.GetService<ICodeSystemDAL>();
            if (codeSystemDAL.GetAllCount() > 0) 
            {
                return Conflict();
            }

            SqlImporter importer = new SqlImporter(_serviceProvider.GetService<IThesaurusDAL>(),
                _serviceProvider.GetService<IThesaurusTranslationDAL>(),
                _serviceProvider.GetService<IO4CodeableConceptDAL>(),
                _serviceProvider.GetService<ICodeDAL>(),
                codeSystemDAL, _serviceProvider.GetService<IAdministrativeDataDAL>(),
                configuration);
            importer.ImportCodingSystems();

            return Ok();
        }


        public ActionResult InsertThesaurusesIntoSql()
        {
            IThesaurusDAL thesaurusDAL = _serviceProvider.GetService<IThesaurusDAL>();
            if (thesaurusDAL.GetAllCount() > 0)
            {
                return Conflict();
            }


            SqlImporter importer = new SqlImporter(thesaurusDAL,
                _serviceProvider.GetService<IThesaurusTranslationDAL>(),
                _serviceProvider.GetService<IO4CodeableConceptDAL>(),
                _serviceProvider.GetService<ICodeDAL>(),
                _serviceProvider.GetService<ICodeSystemDAL>(), _serviceProvider.GetService<IAdministrativeDataDAL>(),
                configuration);
            importer.Import();

            return Ok();
        }
    }
}