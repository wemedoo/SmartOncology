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

namespace sReportsV2.Controllers
{
    public class ImportDataController : Controller
    {
        readonly IThesaurusDAL thesaurusDAL;
        readonly ICodeSystemDAL codeSystemDAL;
        readonly ICodeDAL codeDAL;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration configuration;

        public ImportDataController(IThesaurusDAL thesaurusDAL, ICodeDAL codeDAL, ICodeSystemDAL codeSystemDAL, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.thesaurusDAL = thesaurusDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.codeDAL = codeDAL;
            _serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public ActionResult InsertOrganizations()
        {
            ImportOrganization importer = new ImportOrganization(_serviceProvider.GetService<IOrganizationDAL>());
            int? countryCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.Country, ResourceTypes.CompanyCountry);
            importer.InsertOrganization(DirectoryHelper.AppDataFolder + "\\SwissHospitals.csv", countryCD);
            return Ok();
        }

        public ActionResult InsertCodingSystems()
        {
            if (codeSystemDAL.GetAllCount() > 0) 
            {
                return Conflict();
            }

            SqlImporter importer = new SqlImporter(_serviceProvider.GetService<IThesaurusDAL>(),
                _serviceProvider.GetService<IThesaurusTranslationDAL>(),
                _serviceProvider.GetService<IO4CodeableConceptDAL>(),
                _serviceProvider.GetService<ICodeDAL>(),
                _serviceProvider.GetService<ICodeSystemDAL>(), _serviceProvider.GetService<IAdministrativeDataDAL>(),
                configuration);
            importer.ImportCodingSystems();

            return Ok();
        }


        public ActionResult InsertThesaurusesIntoSql()
        {
            if (thesaurusDAL.GetAllCount() > 0)
            {
                return Conflict();
            }


            SqlImporter importer = new SqlImporter(_serviceProvider.GetService<IThesaurusDAL>(),
                _serviceProvider.GetService<IThesaurusTranslationDAL>(),
                _serviceProvider.GetService<IO4CodeableConceptDAL>(),
                _serviceProvider.GetService<ICodeDAL>(),
                _serviceProvider.GetService<ICodeSystemDAL>(), _serviceProvider.GetService<IAdministrativeDataDAL>(),
                configuration);
            importer.Import();

            return Ok();
        }


        /* private ThesaurusService thesaurusService;

         public ImportDataController()
         {
             thesaurusService = new ThesaurusService();
         }

         public ActionResult ImportLoinc() {
             LoincParser.LoincParser.LoincParser parser = new LoincParser.LoincParser.LoincParser();
             parser.ImportThesaurusesAndFormsFromLoinc(BlobStorageHelper.GetUrl("loinc"));

             return Ok();
         }
         public async Task InsertThesaurusEntriesFromUMLS()
         {
             Importer importer = new Importer();
             await Task.Run(() =>
             {
                 importer.Import(BlobStorageHelper.GetUrl("UMLS"));

             }).ConfigureAwait(false);
         }

         public  ActionResult InsertThesaurusesIntoSql()
         {
             if (thesaurusService.GetAllCount() > 0)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.Conflict);
             }
             CodeSystemService service = new CodeSystemService();
             var codingSystems = service.GetAll(); 

             SqlImporter importer = new SqlImporter();
             importer.Import(BlobStorageHelper.GetUrl("UMLS"));
             return new HttpStatusCodeResult(HttpStatusCode.OK);
         }

         public ActionResult Test()
         {
             //List<ThesaurusEntry> thesauruses = thesaurusService
             var thesaurus = thesaurusService.GetById(493317);
             thesaurusService.Insert(new ThesaurusEntry()
             {
                 Translations = new List<ThesaurusEntryTranslation>()
                 {
                     new ThesaurusEntryTranslation()
                     {
                         Synonyms = new List<string>()
                         {
                             "s1",
                             "s2",
                             "s3"
                         },
                         Abbreviations =  new List<string>()
                         {
                             "a1",
                             "a2",
                             "a3"
                         }
                     }

                 }
             });

             return new HttpStatusCodeResult(HttpStatusCode.OK);
         }

         public ActionResult ExportToCsv()
         {
             CsvExporter exporter = new CsvExporter();
             exporter.SetTermsCsv(System.IO.File.ReadAllLines($"{AppDomain.CurrentDomain.GetData("DataDirectory").ToString()}/terms.txt").ToList());
             exporter.Import(BlobStorageHelper.GetUrl("UMLS"));
             var sb = new StringBuilder();


             sb.AppendLine("Ui, Name, Definition, Atoms, Semantic Types, Term");
             foreach (var csvEntity in exporter.GetCsvEntities())
             {
                 sb.AppendLine(csvEntity.UI + "," + "\"" + csvEntity.Name + "\"" + "," + "\"" + csvEntity.Definition + "\"" + "," + "\"" + csvEntity.Atoms + "\"" + "," + "\"" + csvEntity.SemanticType + "\"" + "," + "\"" + csvEntity.Term + "\"");
             } 

             return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv");
         }

         public string GetAtomsAsString(List<string> atoms) 
         {
             string result = string.Empty;
             foreach (string atom in atoms) 
             {
                 result += atom + "\n";
             }

             return result;
         }

         public void InsertIdentifiers()
         {
             IdentifierInitializer initializer = new IdentifierInitializer();
             initializer.InitializeIdentifiers(initializer.GetOrganizationIdentifierList(), IdentifierKind.OrganizationIdentifierType);
             initializer.InitializeIdentifiers(initializer.GetPatientIdentifierList(), IdentifierKind.PatientIdentifierType);
         }

         public async Task InsertPatients()
         {
             ImportPatient importPatient = new ImportPatient();
             WebClient webClient = new WebClient();
             webClient.Encoding = Encoding.UTF8;

             await Task.Run(() =>
                 {
                     foreach (string patientUrl in BlobStorageHelper.GetUrls("fhirPatients"))
                         importPatient.LoadFromJson(webClient.DownloadString(patientUrl), importPatient.userData);
                 }).ConfigureAwait(false);

         }

         public void InsertOrganizations() 
         {
             ImportOrganization importOrganization = new ImportOrganization();
             importOrganization.InsertOrganization(AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\SwissHospitals.csv");
         } 
         */
    }
}