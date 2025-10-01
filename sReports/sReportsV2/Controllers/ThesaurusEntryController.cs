using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut;
using System.Threading.Tasks;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.Common.DTO;
using System.Collections.Generic;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Autocomplete;
using System.Linq;
using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;

namespace sReportsV2.Controllers
{
    public partial class ThesaurusEntryController : BaseController
    {
        private readonly IFormBLL formBLL;
        private readonly IThesaurusEntryBLL thesaurusEntryBLL;
        private readonly IMapper mapper;

        public ThesaurusEntryController(IThesaurusEntryBLL thesaurusEntryBLL,
            IAsyncRunner asyncRunner, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            ICacheRefreshService cacheRefreshService,
            IFormBLL formBLL) : 
            base(httpContextAccessor, serviceProvider, configuration, asyncRunner, cacheRefreshService)
        {
            this.formBLL = formBLL;
            this.thesaurusEntryBLL = thesaurusEntryBLL;
            this.mapper = mapper;
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
        public ActionResult GetExtensions(string name, List<AutocompleteDataOut> allConceptSchemes)
        {
            name = name ?? string.Empty;
            return PartialView(
                "~/Views/Form/DragAndDrop/CustomFields/AutocompleteValues.cshtml",
                new CustomAutocompleteDataOut()
                {
                    ComponentName = "extension",
                    Options = allConceptSchemes.Where(x => x.text.ToLower().StartsWith(name.ToLower())).ToDictionary(x => x.id, x => x.text)
                }
            );
        }

        [SReportsAuthorize]
        public ActionResult GetNarrowerConcepts(string name)
        {
            return PartialView(
                "~/Views/Form/DragAndDrop/CustomFields/AutocompleteValues.cshtml",
                new CustomAutocompleteDataOut()
                {
                    ComponentName = "narrowerConcept",
                    Options = thesaurusEntryBLL.GetAutocompleteData(name, userCookieData.ActiveLanguage),
                }
            );
        }

        [SReportsAuthorize]
        public ActionResult GetBroaderConcepts(string name)
        {
            return PartialView(
                "~/Views/Form/DragAndDrop/CustomFields/AutocompleteValues.cshtml",
                new CustomAutocompleteDataOut()
                {
                    ComponentName = "broaderConcept",
                    Options = thesaurusEntryBLL.GetAutocompleteData(name, userCookieData.ActiveLanguage),
                }
            );
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
        public async Task<ActionResult> GetReviewTree(ThesaurusReviewFilterDataIn filter)
        {
            ThesaurusEntryDataOut thesaurus = await thesaurusEntryBLL.GetById(filter.Id).ConfigureAwait(false);

            ViewBag.O4MTId = thesaurus.Id;
            ViewBag.Id = filter.Id;
            ViewBag.FilterData = filter;
            ViewBag.PreferredTerm = thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);

            return View("ReviewTree", GetReviewTreeDataOut(filter, thesaurus));
        }

        [SReportsAuthorize]
        public async Task<ActionResult> GetThesaurusInfo(int id)
        {
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);
            return PartialView("ThesaurusInfo", await thesaurusEntryBLL.GetById(id).ConfigureAwait(false));
        }

        [SReportsAuthorize]
        public async Task<ActionResult> ReloadReviewTree(ThesaurusReviewFilterDataIn filter)
        {
            ThesaurusEntryDataOut thesaurus = await thesaurusEntryBLL.GetById(filter.Id).ConfigureAwait(false);

            ViewBag.O4MTId = thesaurus.Id;
            ViewBag.PreferredTerm = string.IsNullOrWhiteSpace(filter.PreferredTerm) ? thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage) : filter.PreferredTerm;
            ViewBag.Id = filter.Id;
            ViewBag.FilterData = filter;

            return PartialView("ThesaurusReviewList", GetReviewTreeDataOut(filter, thesaurus));
        }

        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Thesaurus)]
        public async Task<ActionResult> Create()
        {
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);
            return View(EndpointConstants.Edit, await thesaurusEntryBLL.GetDefaultViewModel().ConfigureAwait(false));
        }

        //done
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Thesaurus)]
        public Task<ActionResult> Edit(int thesaurusEntryId)
        {
            return GetThesaurusEditById(thesaurusEntryId);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public Task<ActionResult> View(int thesaurusEntryId)
        {
            return GetThesaurusEditById(thesaurusEntryId);
        }

        //done
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public Task<ActionResult> EditByO4MtId(int id)
        {
            SetReadOnlyAndDisabledViewBag(true);

            return GetThesaurusEditById(id);
        }

        //done
        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Create, Module = ModuleNames.Thesaurus)]
        [SReportsModelStateValidate]
        public Task<ActionResult> Create(ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            return CreateOrEdit(thesaurusEntryDTO);
        }

        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Thesaurus)]
        [SReportsModelStateValidate]
        public Task<ActionResult> Edit(ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            return CreateOrEdit(thesaurusEntryDTO);
        }

        //done
        [HttpPost]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Update, Module = ModuleNames.Designer)]
        public async Task<ActionResult> CreateByPreferredTerm(string preferredTerm, string description)
        {
            ThesaurusEntry thesaurusEntry = new ThesaurusEntry()
            {
                StateCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.ThesaurusState, ResourceTypes.DefaultThesaurusState),
                Translations = new List<ThesaurusEntryTranslation>()
            };
            thesaurusEntry.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, preferredTerm, description);

            ResourceCreatedDTO result = await thesaurusEntryBLL.CreateThesaurus(mapper.Map<ThesaurusEntryDataIn>(thesaurusEntry), mapper.Map<UserData>(userCookieData)).ConfigureAwait(false);
            RefreshCache(int.Parse(result.Id), ModifiedResourceType.Thesaurus);

            return Json(result);
        }


        //done
        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Thesaurus)]
        [HttpDelete]
        [SReportsAuditLog]
        public async Task<ActionResult> Delete(int thesaurusEntryId)
        {
            await thesaurusEntryBLL.TryDelete(thesaurusEntryId).ConfigureAwait(false);
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

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public async Task<ActionResult> ThesaurusMoreContent(int id)
        {
            return PartialView("ThesaurusMoreContent", await thesaurusEntryBLL.GetById(id).ConfigureAwait(false));
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

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Thesaurus)]
        public async Task<ActionResult> ExportSkos(int thesaurusEntryId)
        {
            ThesaurusEntryDataOut entity = thesaurusEntryBLL.GetThesaurusDataOut(thesaurusEntryId);
            var result = await thesaurusEntryBLL.ExportSkos(thesaurusEntryId).ConfigureAwait(false);
            SetFileNameInResponse(entity.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage), "json");
            return Json(result);
        }

        //public ActionResult PopulateThesaurusEntriesFromForm(string formId)
        //{
        //    List<Form> forms;
        //    if (!string.IsNullOrEmpty(formId))
        //    {
        //        forms = formService.GetByFormIdsList(new List<string>() { formId });

        //        foreach (Form f in forms)
        //        {
        //            ThesaurusEntry thesaurusEntry = new ThesaurusEntry()
        //            {
        //                Translations = new List<ThesaurusEntryTranslation>()
        //            };
        //            thesaurusEntry.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, f.Title, string.Empty);

        //            var formTitle = thesaurusEntryBLL.GetByPreferredTerm(f.Title);
        //            if (formTitle != null)
        //            {
        //                f.ThesaurusId = formTitle.ThesaurusEntryId;
        //            }
        //            else
        //            {
        //                var formResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(thesaurusEntry), Mapper.Map<UserData>(userCookieData));
        //                f.ThesaurusId = int.Parse(formResult.Id);
        //            }

        //            foreach (FormChapter c in f.Chapters)
        //            {
        //                ThesaurusEntry cThesaurus = new ThesaurusEntry()
        //                {
        //                    Translations = new List<ThesaurusEntryTranslation>()
        //                };
        //                cThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, c.Title, c.Description);


        //                var chapterTitle = thesaurusEntryBLL.GetByPreferredTerm(c.Title);
        //                if (chapterTitle != null)
        //                {
        //                    c.ThesaurusId = chapterTitle.ThesaurusEntryId;
        //                }
        //                else
        //                {
        //                    var chapterResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(cThesaurus), Mapper.Map<UserData>(userCookieData));
        //                    c.ThesaurusId = int.Parse(chapterResult.Id);
        //                }

        //                foreach (FormPage p in c.Pages)
        //                {
        //                    ThesaurusEntry pThesaurus = new ThesaurusEntry()
        //                    {
        //                        Translations = new List<ThesaurusEntryTranslation>()
        //                    };
        //                    pThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, p.Title, p.Description);

        //                    var pageTitle = thesaurusEntryBLL.GetByPreferredTerm(p.Title);
        //                    if (pageTitle != null)
        //                    {
        //                        p.ThesaurusId = pageTitle.ThesaurusEntryId;
        //                    }
        //                    else
        //                    {
        //                        var pResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(pThesaurus), Mapper.Map<UserData>(userCookieData));
        //                        p.ThesaurusId = int.Parse(pResult.Id);
        //                    }

        //                    foreach (List<FieldSet> listOfFS in p.ListOfFieldSets)
        //                    {
        //                        foreach (FieldSet fieldSet in listOfFS)
        //                        {
        //                            ThesaurusEntry fieldSetThesaurus = new ThesaurusEntry()
        //                            {
        //                                Translations = new List<ThesaurusEntryTranslation>()
        //                            };
        //                            fieldSetThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, fieldSet.Label, fieldSet.Description);

        //                            var fieldSetTitle = thesaurusEntryBLL.GetByPreferredTerm(fieldSet.Label);
        //                            if (fieldSetTitle != null)
        //                            {
        //                                fieldSet.ThesaurusId = fieldSetTitle.ThesaurusEntryId;
        //                            }
        //                            else
        //                            {
        //                                var fieldSetResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(fieldSetThesaurus), Mapper.Map<UserData>(userCookieData));
        //                                fieldSet.ThesaurusId = int.Parse(fieldSetResult.Id);
        //                            }
        //                            foreach (Field field in fieldSet.Fields)
        //                            {
        //                                ThesaurusEntry fieldThesaurus = new ThesaurusEntry()
        //                                {
        //                                    Translations = new List<ThesaurusEntryTranslation>()
        //                                };
        //                                fieldThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, field.Label, field.Description);

        //                                var fieldTitle = thesaurusEntryBLL.GetByPreferredTerm(field.Label);
        //                                if (fieldTitle != null)
        //                                {
        //                                    field.ThesaurusId = fieldTitle.ThesaurusEntryId;
        //                                }
        //                                else
        //                                {
        //                                    var fieldResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(fieldThesaurus), Mapper.Map<UserData>(userCookieData));
        //                                    field.ThesaurusId = int.Parse(fieldResult.Id);
        //                                }

        //                                if (field is FieldSelectable fieldSelectable)
        //                                {
        //                                    foreach (FormFieldValue formFieldValue in fieldSelectable.Values)
        //                                    {
        //                                        ThesaurusEntry formFieldValueThesaurus = new ThesaurusEntry()
        //                                        {
        //                                            Translations = new List<ThesaurusEntryTranslation>()
        //                                        };
        //                                        formFieldValueThesaurus.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, formFieldValue.Label, string.Empty);

        //                                        var formFieldValueTitle = thesaurusEntryBLL.GetByPreferredTerm(formFieldValue.Label);
        //                                        if (formFieldValueTitle != null)
        //                                        {
        //                                            formFieldValue.ThesaurusId = formFieldValueTitle.ThesaurusEntryId;
        //                                        }
        //                                        else
        //                                        {
        //                                            var fieldValueResult = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(formFieldValueThesaurus), Mapper.Map<UserData>(userCookieData));
        //                                            formFieldValue.ThesaurusId = int.Parse(fieldValueResult.Id);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }

        //                    }
        //                }
        //            }
        //            formService.InsertOrUpdate(f, Mapper.Map<UserData>(userCookieData));
        //        }
        //    }
        //    return null;
        //}

        private PaginationDataOut<ThesaurusEntryDataOut, DataIn> GetReviewTreeDataOut(ThesaurusReviewFilterDataIn filter, ThesaurusEntryDataOut thesaurus)
        {
            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = thesaurusEntryBLL.GetReviewTreeDataOut(filter, thesaurus, userCookieData);
            ViewBag.Thesaurus = thesaurus;
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return result;
        }

        private async Task<ActionResult> GetThesaurusEditById(int thesaurusEntryId)
        {
            if (!thesaurusEntryBLL.ExistsThesaurusEntry(thesaurusEntryId))
            {
                return NotFound();
            }

            ThesaurusEntryDataOut viewModel = await thesaurusEntryBLL.GetById(thesaurusEntryId, userCookieData).ConfigureAwait(false);

            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.TotalAppeareance = formBLL.GetThesaurusAppereanceCount(viewModel.Id, string.Empty, userCookieData.ActiveOrganization);
            ViewBag.VersionTypes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.VersionType);
            ViewBag.ThesaurusStates = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ThesaurusState);

            return View(EndpointConstants.Edit, viewModel);
        }

        private async Task<ActionResult> CreateOrEdit(ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            thesaurusEntryDTO = Ensure.IsNotNull(thesaurusEntryDTO, nameof(thesaurusEntryDTO));
            if (thesaurusEntryDTO.Translations.Count > 0)
                thesaurusEntryDTO.Translations = DecodePreferredTerm(thesaurusEntryDTO.Translations);

            ResourceCreatedDTO result = await thesaurusEntryBLL.CreateThesaurus(thesaurusEntryDTO, mapper.Map<UserData>(userCookieData)).ConfigureAwait(false);
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