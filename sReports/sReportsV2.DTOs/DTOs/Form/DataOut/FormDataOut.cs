using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormDataOut
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
        [DataList]
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
        public DocumentLoincPropertiesDataOut DocumentLoincProperties { get; set; }

        [DataProp]
        public FormEpisodeOfCareDataDataOut EpisodeOfCare { get; set; }
        public List<FormStatusDataOut> WorkflowHistory { get; set; }
        public List<ReferralInfoDTO> ReferrableFields { get; set; }
        [DataProp]
        public bool DisablePatientData { get; set; }
        [DataProp]
        public string OomniaId { get; set; }
        [DataProp]
        public bool AvailableForTask { get; set; }        
        [DataProp]
        public List<int> NullFlavors { get; set; }
        [DataProp]
        public List<int> OrganizationIds { get; set; }
        private List<CustomHeaderFieldDataOut> customHeaderFields = new List<CustomHeaderFieldDataOut>();

        public List<CustomHeaderFieldDataOut> CustomHeaderFields
        {
            get => customHeaderFields;
            set => customHeaderFields = value ?? new List<CustomHeaderFieldDataOut>();
        }

        public FormDataOut()
        {
            WorkflowHistory = new List<FormStatusDataOut>();
        }

        #region Designer
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

        public object GetCustomMarkedPropertiesObject()
        {
            return GetCustomMarkedPropertiesObject(this);
        }

        public List<LoincPropertyAutocompleteDataOut> GetAutocompleteHierarchyData(string fieldId)
        {
            List<LoincPropertyAutocompleteDataOut> loincPropertyPerName = new List<LoincPropertyAutocompleteDataOut>();

            foreach (FormChapterDataOut chapter in Chapters)
            {
                AddAutocompleteOption(loincPropertyPerName, chapter.Id, chapter.Title, 0);
                foreach (FormPageDataOut page in chapter.Pages)
                {
                    AddAutocompleteOption(loincPropertyPerName, page.Id, page.Title, 1);
                    foreach (FormFieldSetDataOut fieldSet in page.ListOfFieldSets.SelectMany(p => p))
                    {
                        AddAutocompleteOption(loincPropertyPerName, fieldSet.Id, fieldSet.Label, 2);
                        foreach (FieldDataOut field in fieldSet.Fields)
                        {
                            if (field.Id != fieldId && field.CanBeConnectedField())
                            {
                                AddAutocompleteOption(loincPropertyPerName, field.Id, field.Label, 3);
                            }
                        }
                    }
                }

            }

            return loincPropertyPerName;
        }

        private void AddAutocompleteOption(List<LoincPropertyAutocompleteDataOut> loincPropertyPerName, string id, string title, int level)
        {
            loincPropertyPerName.Add(new LoincPropertyAutocompleteDataOut
            {
                id = id,
                level = level.ToString(),
                text = title
            });
        }

        private object GetCustomMarkedPropertiesObject(object source)
        {
            Type objectType = source.GetType();
            PropertyInfo[] allProperties = objectType
            .GetProperties();

            Dictionary<string, object> propertiesWithDataProp = allProperties
                .Where(p => Attribute.IsDefined(p, typeof(DataPropAttribute)))
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(source)
                );

            // Return as anonymous object
            var anon = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            foreach (var kvp in propertiesWithDataProp)
            {
                anon[kvp.Key] = kvp.Value;
            }

            Dictionary<string, object> propertiesWithDataList = allProperties
                .Where(p => Attribute.IsDefined(p, typeof(DataListAttribute)))
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(source)
                );

            foreach (var kvp in propertiesWithDataList)
            {
                string propName = kvp.Key;
                if (kvp.Value is System.Collections.IEnumerable enumerable)
                {
                    var list = new List<object>();
                    foreach (var item in enumerable)
                    {
                        if (item is System.Collections.IEnumerable enumerableOfEnumerable)
                        {
                            var subList = new List<object>();
                            foreach (var subItem in enumerableOfEnumerable)
                            {
                                subList.Add(GetCustomMarkedPropertiesObject(subItem));
                            }
                            list.Add(subList);
                        }
                        else
                        {
                            list.Add(GetCustomMarkedPropertiesObject(item));
                        }
                    }
                    anon.Add(propName, list);
                }
            }

            return anon;
        }


        #endregion /Designer
    }
}