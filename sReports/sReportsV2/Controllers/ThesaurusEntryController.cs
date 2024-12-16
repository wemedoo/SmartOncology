using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System.Collections.Generic;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.Common.Enums;
using System;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut;

namespace sReportsV2.Controllers
{
    public partial class ThesaurusEntryController : BaseController
    {
        private readonly IFormDAL formService;
        private readonly IThesaurusEntryBLL thesaurusEntryBLL;
        private readonly IMapper Mapper;

        public ThesaurusEntryController(IThesaurusEntryBLL thesaurusEntryBLL, IAsyncRunner asyncRunner, IMapper mapper, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IConfiguration configuration) : base(httpContextAccessor, serviceProvider, configuration, asyncRunner)
        {
            this.formService = new FormDAL();
            this.thesaurusEntryBLL = thesaurusEntryBLL;
            Mapper = mapper;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public ActionResult GetAll(ThesaurusEntryFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return View();
        }

        //done
        [SReportsAuthorize]
        public ActionResult ReloadTable(ThesaurusEntryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            PaginationDataOut<ThesaurusEntryViewDataOut, DataIn> result = thesaurusEntryBLL.ReloadTable(dataIn);
            return PartialView("ThesaurusEntryTable", result);
        }

        [SReportsAuthorize]
        public ActionResult ThesaurusProperties(int? o4mtid)
        {
            ThesaurusEntryDataOut viewModel = this.thesaurusEntryBLL.GetThesaurusDataOut(o4mtid.Value);
            return PartialView(viewModel);
        }

        //done
        [SReportsAuthorize]
        public ActionResult ReloadSearchTable(ThesaurusEntryFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.ActiveLanguage = userCookieData.ActiveLanguage;
            dataIn.IsSearchTable = true;
            dataIn.PreferredTerm = System.Net.WebUtility.UrlDecode(dataIn.PreferredTerm);
            PaginationDataOut<ThesaurusEntryViewDataOut, DataIn> result = thesaurusEntryBLL.ReloadTable(dataIn);
            ViewBag.ActiveThesaurus = dataIn.ActiveThesaurus;
            return PartialView(result);
        }

        [Authorize]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public ActionResult GetReviewTree(ThesaurusReviewFilterDataIn filter)
        {
            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus = thesaurusEntryBLL.GetById(filter.Id);

            ViewBag.O4MTId = thesaurus.ThesaurusEntryId;
            ViewBag.Id = filter.Id;
            ViewBag.CurrentThesaurus = thesaurus;
            ViewBag.FilterData = filter;
            ViewBag.PreferredTerm = thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);

            return View("ReviewTree", GetReviewTreeDataOut(filter, thesaurus));
        }

        [SReportsAuthorize]
        public ActionResult GetThesaurusInfo(int id)
        {
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);
            return PartialView("ThesaurusInfo", Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntryBLL.GetById(id)));
        }

        [SReportsAuthorize]
        public ActionResult ReloadReviewTree(ThesaurusReviewFilterDataIn filter)
        {
            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus = thesaurusEntryBLL.GetById(filter.Id);

            ViewBag.O4MTId = thesaurus.ThesaurusEntryId;
            ViewBag.PreferredTerm = string.IsNullOrWhiteSpace(filter.PreferredTerm) ? thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage) : filter.PreferredTerm;
            ViewBag.Id = filter.Id;
            ViewBag.FilterData = filter;

            return PartialView("ThesaurusReviewList", GetReviewTreeDataOut(filter, thesaurus));
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Thesaurus)]
        public ActionResult Create()
        {
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);
            return View(EndpointConstants.Edit);
        }

        //done
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Thesaurus)]
        public ActionResult Edit(int thesaurusEntryId)
        {
            return GetThesaurusEditById(thesaurusEntryId);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public ActionResult View(int thesaurusEntryId)
        {
            return GetThesaurusEditById(thesaurusEntryId);
        }

        //done
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public ActionResult EditByO4MtId(int id)
        {
            SetReadOnlyAndDisabledViewBag(true);

            return GetThesaurusEditById(id);
        }

        //done
        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Thesaurus)]
        [SReportsModelStateValidate]
        public ActionResult Create(ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            thesaurusEntryDTO = Ensure.IsNotNull(thesaurusEntryDTO, nameof(thesaurusEntryDTO));
            return CreateOrEdit(thesaurusEntryDTO);
        }

        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Thesaurus)]
        [SReportsModelStateValidate]
        public ActionResult Edit(ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            thesaurusEntryDTO = Ensure.IsNotNull(thesaurusEntryDTO, nameof(thesaurusEntryDTO));
            return CreateOrEdit(thesaurusEntryDTO);
        }

        //done
        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public ActionResult CreateByPreferredTerm(string preferredTerm, string description)
        {
            ThesaurusEntry thesaurusEntry = new ThesaurusEntry()
            {
                StateCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.ThesaurusState, ResourceTypes.DefaultThesaurusState),
                Translations = new List<ThesaurusEntryTranslation>()
            };
            thesaurusEntry.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, preferredTerm, description);

            ResourceCreatedDTO result = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(thesaurusEntry), Mapper.Map<UserData>(userCookieData));
            RefreshCache(int.Parse(result.Id), ModifiedResourceType.Thesaurus);

            return Json(result);
        }


        //done
        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Thesaurus)]
        [HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(int thesaurusEntryId)
        {
            thesaurusEntryBLL.TryDelete(thesaurusEntryId);
            return NoContent();
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateCode, Module = ModuleNames.Thesaurus)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult CreateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId)
        {
            var viewModel = thesaurusEntryBLL.InsertOrUpdateCode(codeDataIn, thesaurusEntryId);
            SetReadOnlyAndDisabledViewBag(false);
            return PartialView("CodeRow", viewModel);
        }


        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Thesaurus)]
        [HttpDelete]
        public ActionResult DeleteCode(int codeId)
        {
            thesaurusEntryBLL.DeleteCode(codeId);

            return Json("Code deleted");
        }

        /* public ActionResult LoadTranslations()
         {
             UserData userData = Mapper.Map<UserData>(userCookieData);
             ParserThesaurusTranslation.ParseAndUpdateThesaurus(userData);

             return new HttpStatusCodeResult(HttpStatusCode.NoContent);
         }*/
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public ActionResult ThesaurusMoreContent(int id)
        {
            ThesaurusEntryDataOut viewModel = Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntryBLL.GetById(id));

            return PartialView("ThesaurusMoreContent", viewModel);
        }


        [SReportsAuthorize]
        public ActionResult InsertNewThesaurusFromForm()
        {
            return View();
        }

        //done
        public ActionResult GetEntriesCount()
        {
            ThesaurusEntryCountDataOut result = thesaurusEntryBLL.GetEntriesCount();
            return Json(result);
        }

        [SReportsAuthorize]
        public ActionResult ThesaurusPreview(int? o4mtid, int activeThesaurus)
        {
            ThesaurusEntryDataOut viewModel = thesaurusEntryBLL.GetThesaurusDataOut(o4mtid.Value);
            ViewBag.ActiveThesaurus = activeThesaurus;
            return PartialView(viewModel);
        }

        /*public ActionResult PopulateThesaurusEntriesFromForm(string formId)
        {
            List<Form> forms;
            if (string.IsNullOrEmpty(formId))
            {
                forms = formService.GetAll(null);
            }
            else
            {
                forms = formService.GetByFormIdsList(new List<string>() { formId });
            }
            foreach (Form f in forms)
            {
                ThesaurusEntry thesaurusEntry = new ThesaurusEntry()
                {
                    Translations = new List<ThesaurusEntryTranslation>()
                };
                thesaurusEntry.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, f.Title, string.Empty);
                var formResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(thesaurusEntry), Mapper.Map<UserData>(userCookieData));
                f.ThesaurusId = int.Parse(formResult.Id);

                foreach (FormChapter c in f.Chapters)
                {
                    ThesaurusEntry cThesaurus = new ThesaurusEntry()
                    {
                        Translations = new List<ThesaurusEntryTranslation>()
                    };
                    cThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, c.Title, c.Description);
                    var chapterResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(cThesaurus), Mapper.Map<UserData>(userCookieData));
                    c.ThesaurusId = int.Parse(chapterResult.Id);

                    foreach (FormPage p in c.Pages)
                    {
                        ThesaurusEntry pThesaurus = new ThesaurusEntry()
                        {
                            Translations = new List<ThesaurusEntryTranslation>()
                        };
                        pThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, p.Title, p.Description);
                        var pResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(pThesaurus), Mapper.Map<UserData>(userCookieData));
                        p.ThesaurusId = int.Parse(pResult.Id);

                        foreach (List<FieldSet> listOfFS in p.ListOfFieldSets)
                        {
                            foreach (FieldSet fieldSet in listOfFS)
                            {
                                ThesaurusEntry fieldSetThesaurus = new ThesaurusEntry()
                                {
                                    Translations = new List<ThesaurusEntryTranslation>()
                                };
                                fieldSetThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, fieldSet.Label, fieldSet.Description);
                                var fieldSetResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(fieldSetThesaurus), Mapper.Map<UserData>(userCookieData));
                                fieldSet.ThesaurusId = int.Parse(fieldSetResult.Id);

                                foreach (Field field in fieldSet.Fields)
                                {
                                    ThesaurusEntry fieldThesaurus = new ThesaurusEntry()
                                    {
                                        Translations = new List<ThesaurusEntryTranslation>()
                                    };
                                    fieldThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, field.Label, field.Description);
                                    var fieldResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(fieldThesaurus), Mapper.Map<UserData>(userCookieData));
                                    field.ThesaurusId = int.Parse(fieldResult.Id);

                                    if (field is FieldSelectable)
                                    {
                                        FieldSelectable fieldSelectable = field as FieldSelectable;
                                        foreach (FormFieldValue formFieldValue in fieldSelectable.Values)
                                        {
                                            ThesaurusEntry formFieldValueThesaurus = new ThesaurusEntry()
                                            {
                                                Translations = new List<ThesaurusEntryTranslation>()
                                            };
                                            formFieldValueThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, formFieldValue.Label, string.Empty);
                                            var fieldValueResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(formFieldValueThesaurus), Mapper.Map<UserData>(userCookieData));
                                            formFieldValue.ThesaurusId = int.Parse(fieldValueResult.Id);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                formService.InsertOrUpdate(f, Mapper.Map<UserData>(userCookieData));
            }
            return null;
        }*/

        private PaginationDataOut<ThesaurusEntryDataOut, DataIn> GetReviewTreeDataOut(ThesaurusReviewFilterDataIn filter, sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus)
        {
            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = thesaurusEntryBLL.GetReviewTreeDataOut(filter, thesaurus, userCookieData);
            ViewBag.Thesaurus = Mapper.Map<ThesaurusEntryDataOut>(thesaurus);
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return result;
        }

        private ActionResult GetThesaurusEditById(int thesaurusEntryId)
        {
            if (!thesaurusEntryBLL.ExistsThesaurusEntry(thesaurusEntryId))
            {
                return NotFound();
            }

            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurusEntry = thesaurusEntryBLL.GetById(thesaurusEntryId);
            ThesaurusEntryDataOut viewModel = Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntry);
            thesaurusEntryBLL.SetThesaurusVersions(thesaurusEntry, viewModel);


            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.TotalAppeareance = formService.GetThesaurusAppereanceCount(thesaurusEntry.ThesaurusEntryId, string.Empty, userCookieData.ActiveOrganization);
            ViewBag.VersionTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.VersionType);
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return View(EndpointConstants.Edit, viewModel);
        }

        private ActionResult CreateOrEdit(ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            if (thesaurusEntryDTO.Translations.Count > 0)
                thesaurusEntryDTO.Translations = DecodePreferredTerm(thesaurusEntryDTO.Translations);

            ResourceCreatedDTO result = thesaurusEntryBLL.CreateThesaurus(thesaurusEntryDTO, Mapper.Map<UserData>(userCookieData));
            RefreshCache(int.Parse(result.Id), ModifiedResourceType.Thesaurus);
            return Json(result);
        }

        private List<ThesaurusEntryTranslationDataIn> DecodePreferredTerm(List<ThesaurusEntryTranslationDataIn> translations) 
        {
            foreach (var translation in translations)
            {
                translation.PreferredTerm = System.Net.WebUtility.UrlDecode(translation.PreferredTerm);
            }

            return translations;
        }
    }
}