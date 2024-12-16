using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Resources;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.CRF.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Controllers
{
    public class CRFController : FormCommonController
    {

        private readonly List<string> ApprovedLanguages = new List<string>() { LanguageConstants.DE, LanguageConstants.FR, LanguageConstants.SR, LanguageConstants.SR_CYRL_RS, LanguageConstants.EN, LanguageConstants.RU, LanguageConstants.ES, LanguageConstants.PT };
        // GET: CRF

        public CRFController(IPatientDAL patientDAL, 
            IEpisodeOfCareDAL episodeOfCareDAL,
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICodeBLL codeBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IEncounterDAL encounterDAL, 
            IThesaurusDAL thesaurusDAL, 
            IAsyncRunner asyncRunner, 
            IPdfBLL pdfBLL, 
            IMapper mapper,             
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider,
            IConfiguration configuration) :
            base(patientDAL, episodeOfCareDAL,encounterDAL,userBLL, organizationBLL, codeBLL, formInstanceBLL, formBLL, thesaurusDAL, asyncRunner, pdfBLL, mapper, httpContextAccessor, serviceProvider, configuration)
        {

        }
        public ActionResult Create(int id, string language = LanguageConstants.EN)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                id = 14573;
                language = LanguageConstants.EN;
            }
            Form form = formDAL.GetFormByThesaurusAndLanguage(id, language);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, id.ToString());
            }

            FormDataOut data = formBLL.SetFormDependablesAndReferrals(form, null, userCookieData);

            Form form1 = form.ThesaurusId == 14573? form: formDAL.GetFormByThesaurusAndLanguage(14573, language);
            Form form2 = form.ThesaurusId == 14911 ? form : formDAL.GetFormByThesaurusAndLanguage(14911, language);
            Form form3 = form.ThesaurusId == 15112 ? form : formDAL.GetFormByThesaurusAndLanguage(15112, language);

            List<Form> formsForTree = new List<Form>();
            formsForTree.Add(form1);
            formsForTree.Add(form2);
            formsForTree.Add(form3);


            SetApprovedLanguages();
            ViewBag.Language = language;
            ViewBag.Tree = GetTreeJson(formsForTree);
            ViewBag.TreeForms = formsForTree;
            ViewBag.MainCreateAction = "crf/create?";
            return View(data);
        }

        private List<TreeJsonDataOut> GetTreeJson(List<Form> formsData)
        {
            List<TreeJsonDataOut> result = new List<TreeJsonDataOut>();
            foreach(Form formData in formsData)
            {
                TreeJsonDataOut treeJsonDataOut = new TreeJsonDataOut();
                treeJsonDataOut.text = formData.Title;
                treeJsonDataOut.nodes = formData.Chapters.Select(x => new TreeJsonDataOut() { text = x.Title, href = $"#@c.Id" }).ToList();

                result.Add(treeJsonDataOut);
            }

            return result;
        }


        public ActionResult Edit(FormInstanceFilterDataIn filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            if (string.IsNullOrWhiteSpace(filter.Language))
            {
                filter.ThesaurusId = 14573;
                filter.Language = LanguageConstants.EN;
            }

            FormInstance formInstance = formInstanceDAL.GetById(filter.FormInstanceId);
            if (formInstance == null)
            {
                return NotFound(TextLanguage.FormInstanceNotExists, filter.FormInstanceId);
            }

            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.Title = formInstance.Title;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.Language = filter.Language;
            ViewBag.ThesaurusId = formInstance.ThesaurusId;
            SetApprovedLanguages();
            return GetEdit(formInstance, filter);
        }

        [HttpPost]
        public async Task<ActionResult> Create(string language, FormInstanceDataIn formInstanceDataIn)
        {
            formInstanceDataIn = Ensure.IsNotNull(formInstanceDataIn, nameof(formInstanceDataIn));
            Form form = this.formDAL.GetForm(formInstanceDataIn.FormDefinitionId);
            if (form == null)
            {
                return NotFound(TextLanguage.FormNotExists, formInstanceDataIn.FormDefinitionId);
            }
            FormInstance formInstance = formInstanceBLL.GetFormInstanceSet(form, formInstanceDataIn, userCookieData);

            await formInstanceBLL.InsertOrUpdateAsync(
                formInstance, 
                formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id),
                userCookieData
                ).ConfigureAwait(false);

            return RedirectToAction("GetAllByFormThesaurus", "FormInstance", new
            {
                thesaurusId = formInstanceDataIn.ThesaurusId,
                formId = form.Id,
                title = form.Title,
                IsSimplifiedLayout = true,
                Language = language
            });
        }

        public ActionResult Instructions(string language)
        {
            ViewBag.Language = language;

            return View();
        }

        private ActionResult GetEdit(FormInstance formInstance, FormInstanceFilterDataIn filter)
        {
            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            Form form = formDAL.GetForm(formInstance.FormDefinitionId);
            form.SetFieldInstances(formInstance.FieldInstances);
            FormDataOut data = formBLL.SetFormDependablesAndReferrals(form, null, userCookieData);
  
            return View("~/Views/CRF/Create.cshtml", data);
        }

        private void SetApprovedLanguages()
        {
            ViewBag.Languages = SingletonDataContainer.Instance.GetLanguages().Where(x => ApprovedLanguages.Contains(x.Value)).OrderBy(x => x.Label).ToList();
        }


    }
}