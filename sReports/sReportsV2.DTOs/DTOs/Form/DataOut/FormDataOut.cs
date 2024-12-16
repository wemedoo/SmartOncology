using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormDataOut
    {
        public static string DefaultIdPlaceholder { get; set; } = "formIdPlaceHolder";
        public static string DefaultFormPlaceholder { get; set; } = "Form title";

        private string _id;
        [DataProp]
        public string Id
        {
            get { return string.IsNullOrWhiteSpace(_id) ? DefaultIdPlaceholder : _id; }
            set { _id = value; }
        }
        [DataProp]
        public FormAboutDataOut About { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        [DataProp]
        public DateTime? EntryDatetime { get; set; }
        [DataProp]
        public DateTime? LastUpdate { get; set; }
        public List<OrganizationDataOut> Organizations { get; set; }
        public List<FormChapterDataOut> Chapters { get; set; } = new List<FormChapterDataOut>();
        [DataProp]
        public FormDefinitionState State { get; set; }
        [DataProp]
        public string Language { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public DocumentPropertiesDataOut DocumentProperties { get; set; }
        [DataProp]
        public FormEpisodeOfCareDataDataOut EpisodeOfCare { get; set; }
        public List<FormStatusDataOut> WorkflowHistory { get; set; }
        public List<ReferralInfoDTO> ReferrableFields { get; set; }
        [DataProp]
        public bool DisablePatientData { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        [DataProp]
        public string OomniaId { get; set; }
        public bool IsParameterize { get; set; }
        [DataProp]
        public bool AvailableForTask { get; set; }
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }
        public string ActiveChapterId { get; set; }
        public int? ActivePageLeftScroll { get; set; }
        public string ActivePageId { get; set; }
        [DataProp]
        public List<int> NullFlavors { get; set; }
        [DataProp]
        public List<int> OrganizationIds { get; set; }
        public List<FieldDataOut> RequiredFieldsWithoutValue { get; set; } = new List<FieldDataOut>();

        private List<CustomHeaderFieldDataOut> customHeaderFields = new List<CustomHeaderFieldDataOut>();
        private Dictionary<string, List<DependentOnInstanceInfoDataOut>> parentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        
        public FormDataOut()
        {
            WorkflowHistory = new List<FormStatusDataOut>();
        }

        public List<CustomHeaderFieldDataOut> CustomHeaderFields
        {
            get => customHeaderFields;
            set => customHeaderFields = value ?? new List<CustomHeaderFieldDataOut>();
        }

        public Dictionary<string, List<DependentOnInstanceInfoDataOut>> ParentFieldInstanceDependencies
        {
            get => parentFieldInstanceDependencies;
            set => parentFieldInstanceDependencies = value ?? new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();
        }

        #region Designer
        public object ToJson()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this, serializerSettings));
        }
        public List<FieldDataOut> GetAllFields()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(list =>
                                    list.SelectMany(set => set.Fields
                                    )
                                )
                            )
                        )
                        .ToList();
        }

        public string GetActiveVersionJsonString()
        {
            return HttpUtility.UrlEncode(JsonConvert.SerializeObject(Version, Formatting.None));
        }

        public static string GetInitialDataAttributes()
        {
            return $"data-title='New Form' data-id='{DefaultIdPlaceholder}'";
        }

        public string GetStateColor(FormDefinitionState status)
        {
            string color = "";
            switch (status)
            {
                case FormDefinitionState.DesignPending:
                    color = "#f7af00";
                    break;
                case FormDefinitionState.Design:
                    color = "#ffa500";
                    break;
                case FormDefinitionState.ReviewPending:
                    color = "#FF0000";
                    break;
                case FormDefinitionState.Review:
                    color = "#aced16";
                    break;
                case FormDefinitionState.ReadyForDataCapture:
                    color = "#daf00d";
                    break;
                case FormDefinitionState.Archive:
                    color = "#bdc6c7";
                    break;
            }
            return color;
        }

        public IEnumerable<FormStatusDataOut> GetWorkflowHistory()
        {
            return WorkflowHistory.OrderByDescending(x => x.Created);
        }

        public bool IsNullFlavorChecked(int codeId)
        {
            return NullFlavors.Contains(codeId);
        }

        #endregion /Designer

        #region Form Instance Methods
        public void SetDoesAllMandatoryFieldsHaveValue()
        {
            foreach (FormPageDataOut page in Chapters.SelectMany(ch => ch.Pages))
            {
                page.SetDoesAllMandatoryFieldsHaveValue();
            }

            foreach (FormChapterDataOut chapter in Chapters)
            {
                chapter.DoesAllMandatoryFieldsHaveValue = chapter.Pages.Select(p => p.DoesAllMandatoryFieldsHaveValue).All(p => p);
            }

            this.DoesAllMandatoryFieldsHaveValue = Chapters.Select(ch => ch.DoesAllMandatoryFieldsHaveValue).All(ch => ch);
        }


        public int CountAllFieldsWhichCanSaveWithoutValue(string chapterId)
        {
            return this.Chapters
                .Where(chapter => chapter.Id == chapterId)
                .SelectMany(chapter => chapter.Pages
                    .SelectMany(page => page.ListOfFieldSets
                        .SelectMany(fieldSet => fieldSet.SelectMany(field => field.Fields
                            .Where(x => x.FieldInstanceValues.Any() && x.FieldInstanceValues.FirstOrDefault()?.Values.Count == 0
                                && x.AllowSaveWithoutValue.HasValue && x.AllowSaveWithoutValue.Value && x.IsVisible))
                        )
                    )
                )
                .Count();
        }


        public List<FormFieldSetDataOut> GetFieldSetDefinitions()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .Select(fieldSet => fieldSet.FirstOrDefault())
                                    .Where(fS => fS != null)
                            )
                        )
                        .ToList();
        }

        public FormFieldSetDataOut GetFieldSet(string fieldSetId)
        {
            return this.Chapters
                       .SelectMany(chapter => chapter.Pages)
                       .SelectMany(page => page.ListOfFieldSets)
                       .SelectMany(innerList => innerList)
                       .FirstOrDefault(fieldSet => fieldSet != null && fieldSet.Id == fieldSetId);
        }

        public List<FormFieldSetDataOut> GetAllFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet)
                            )
                        )
                        .ToList();
        }

        public List<FieldDataOut> GetAllFieldsWhichCannotSaveWithoutValue(List<string> fieldsIds)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(list =>
                                    list.SelectMany(set => set.Fields.Where(y => fieldsIds.Contains(y.Id) && y.AllowSaveWithoutValue.HasValue && !y.AllowSaveWithoutValue.Value)
                                    )
                                )
                            )
                        )
                        .ToList();
        }

        public List<FieldDataOut> GetAllFieldsWhichCanSaveWithoutValue(List<string> fieldsIds)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(list =>
                                    list.SelectMany(set => set.Fields.Where(y => fieldsIds.Contains(y.Id))
                                    )
                                )
                            )
                        )
                        .ToList();
        }
        public void SetIfChaptersAndPagesAreLocked(Dictionary<string, bool> chaptersState, Dictionary<string, bool> pagesState)
        {
            foreach (FormChapterDataOut chapter in Chapters)
            {
                chaptersState.TryGetValue(chapter.Id, out bool isChapterLocked);
                chapter.IsLocked = isChapterLocked;

                bool canBeLockedNext = true;
                foreach (FormPageDataOut page in chapter.Pages)
                {
                    pagesState.TryGetValue(page.Id, out bool isPageLocked);
                    page.IsLocked = isPageLocked;
                    page.CanBeLockedNext = canBeLockedNext;
                    canBeLockedNext &= page.IsLocked;
                }
            }
        }
        public void SetActiveChapterAndPageId(FormInstanceReloadDataIn formInstanceReloadData)
        {
            if (formInstanceReloadData == null)
            {
                formInstanceReloadData = new FormInstanceReloadDataIn();
            }
            if (string.IsNullOrEmpty(formInstanceReloadData?.ActiveChapterId))
            {
                formInstanceReloadData.ActiveChapterId = this.Chapters.FirstOrDefault()?.Id;
            }
            if (string.IsNullOrEmpty(formInstanceReloadData?.ActivePageId))
            {
                formInstanceReloadData.ActivePageId = this.Chapters.FirstOrDefault()?.Pages?.FirstOrDefault()?.Id;
            }
            this.ActiveChapterId = formInstanceReloadData.ActiveChapterId;
            this.ActivePageId = formInstanceReloadData.ActivePageId;
            this.ActivePageLeftScroll = formInstanceReloadData.ActivePageLeftScroll;
        }
        public bool IsFormInstanceLockedOrUnlocked()
        {
            return this.FormState == sReportsV2.Common.Enums.FormState.Locked ||
                this.FormState == sReportsV2.Common.Enums.FormState.Unlocked;
        }

        public bool IsFormInstanceLocked()
        {
            return this.FormState == sReportsV2.Common.Enums.FormState.Locked;
        }

        public bool IsFormInstanceInActiveState()
        {
            return !this.FormState.HasValue || (this.FormState.Equals(sReportsV2.Common.Enums.FormState.OnGoing) || this.FormState.Equals(sReportsV2.Common.Enums.FormState.InError) || this.FormState.Equals(sReportsV2.Common.Enums.FormState.Unlocked));

        }

        public string GetTimeZone()
        {
            return Organizations?.FirstOrDefault()?.TimeZone ?? string.Empty;
        }
        #endregion /Form Instance Methods

        #region Dependency Handling
        public List<FieldInstanceDTO> CreateParentDependableStructure(List<FieldDataOut> populatedFieldInstances)
        {
            List<FieldInstanceDTO> populatedParentFieldInstances = new List<FieldInstanceDTO>();

            ParentFieldInstanceDependencies = new Dictionary<string, List<DependentOnInstanceInfoDataOut>>();

            Dictionary<string, bool> fieldSetRepetitive = GetFieldSetDefinitions().ToDictionary(x => x.Id, x => x.IsRepetitive);
            foreach (FieldDataOut childDependentField in populatedFieldInstances.Where(f => HasDependentOn(f)))
            {
                childDependentField.AddMissingPropertiesInDependency(this);
                DependentOnInfoDataOut dependentOnInfo = childDependentField.DependentOn;
                foreach (var grouping in dependentOnInfo.DependentOnFieldInfos.GroupBy(x => x.FieldId))
                {
                    DependentOnFieldInfoDataOut dependendOnField = grouping.First();
                    FieldDataOut parentField = GetParentField(childDependentField, populatedFieldInstances, dependendOnField);
                    if (parentField != null)
                    {
                        foreach (FieldInstanceValueDataOut parentFieldInstanceValue in parentField.FieldInstanceValues)
                        {
                            populatedParentFieldInstances.Add(new FieldInstanceDTO(parentField, parentFieldInstanceValue));

                            IEnumerable<DependentOnInstanceInfoDataOut> upcomingChildFieldInstanceDependencies = GetUpcomingChildFieldInstanceDependencies(
                                childDependentField,
                                parentFieldInstanceValue.FieldInstanceRepetitionId,
                                fieldSetRepetitive[childDependentField.FieldSetId]
                                );
                            AppendChildDependencies(parentFieldInstanceValue.FieldInstanceRepetitionId, upcomingChildFieldInstanceDependencies);
                        }
                    }
                }
            }

            return populatedParentFieldInstances;

        }

        private FieldDataOut GetParentField(FieldDataOut childDependentField, List<FieldDataOut> populatedFieldInstances, DependentOnFieldInfoDataOut dependendOnField)
        {
            FieldDataOut parentField = populatedFieldInstances.Find(f =>
                f.Id == dependendOnField.FieldId
                && f.FieldSetInstanceRepetitionId == childDependentField.FieldSetInstanceRepetitionId
                )
                ?? populatedFieldInstances.Find(f => f.Id == dependendOnField.FieldId);
            return parentField;
        }

        private IEnumerable<DependentOnInstanceInfoDataOut> GetUpcomingChildFieldInstanceDependencies(FieldDataOut childDependentField, string parentFieldInstanceRepetitionId, bool isChildDependentFieldSetRepetitive)
        {
            return childDependentField
                .FieldInstanceValues
                .Select(x => new DependentOnInstanceInfoDataOut(childDependentField.DependentOn)
                {
                    ChildFieldInstanceRepetitionId = x.FieldInstanceRepetitionId,
                    ChildFieldSetInstanceRepetitionId = childDependentField.FieldSetInstanceRepetitionId,
                    ChildFieldInstanceCssSelector = childDependentField.GetChildFieldInstanceCssSelector(x.FieldInstanceRepetitionId),
                    ParentFieldInstanceCssSelector = childDependentField.GetParentFieldInstanceCssSelector(parentFieldInstanceRepetitionId),
                    IsChildDependentFieldSetRepetitive = isChildDependentFieldSetRepetitive
                });
        }

        private bool HasDependentOn(FieldDataOut fieldDataOut)
        {
            return fieldDataOut.DependentOn != null && fieldDataOut.DependentOn.DependentOnFieldInfos != null && fieldDataOut.DependentOn.DependentOnFieldInfos.Any();
        }

        private void AppendChildDependencies(string parentFieldInstanceRepetitionId, IEnumerable<DependentOnInstanceInfoDataOut> upcomingChildFieldInstanceDependencies)
        {
            if (ParentFieldInstanceDependencies.TryGetValue(parentFieldInstanceRepetitionId, out List<DependentOnInstanceInfoDataOut> childFieldInstanceDependencies))
            {
                childFieldInstanceDependencies.AddRange(upcomingChildFieldInstanceDependencies);
            }
            else
            {
                ParentFieldInstanceDependencies.Add(
                    parentFieldInstanceRepetitionId,
                    new List<DependentOnInstanceInfoDataOut>(upcomingChildFieldInstanceDependencies)
                    );
            }
        }
        #endregion /Dependency Handling
    }
}