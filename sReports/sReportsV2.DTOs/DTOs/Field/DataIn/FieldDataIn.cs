using JsonSubTypes;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataIn.Custom;
using sReportsV2.DTOs.Form.DataIn;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataIn
{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(FieldCalculativeDataIn), FieldTypes.Calculative)]
    [JsonSubtypes.KnownSubType(typeof(FieldCheckboxDataIn), FieldTypes.Checkbox)]
    [JsonSubtypes.KnownSubType(typeof(FieldDateDataIn), FieldTypes.Date)]
    [JsonSubtypes.KnownSubType(typeof(FieldDatetimeDataIn), FieldTypes.Datetime)]
    [JsonSubtypes.KnownSubType(typeof(FieldNumericDataIn), FieldTypes.Number)]
    [JsonSubtypes.KnownSubType(typeof(FieldEmailDataIn), FieldTypes.Email)]
    [JsonSubtypes.KnownSubType(typeof(FieldFileDataIn), FieldTypes.File)]
    [JsonSubtypes.KnownSubType(typeof(FieldTextAreaDataIn), FieldTypes.LongText)]
    [JsonSubtypes.KnownSubType(typeof(FieldRadioDataIn), FieldTypes.Radio)]
    [JsonSubtypes.KnownSubType(typeof(FieldRegexDataIn), FieldTypes.Regex)]
    [JsonSubtypes.KnownSubType(typeof(FieldSelectDataIn), FieldTypes.Select)]
    [JsonSubtypes.KnownSubType(typeof(FieldTextDataIn), FieldTypes.Text)]
    [JsonSubtypes.KnownSubType(typeof(CustomFieldButtonDataIn), FieldTypes.CustomButton)]
    [JsonSubtypes.KnownSubType(typeof(FieldCodedDataIn), FieldTypes.Coded)]
    [JsonSubtypes.KnownSubType(typeof(FieldParagraphDataIn), FieldTypes.Paragraph)]
    [JsonSubtypes.KnownSubType(typeof(FieldLinkDataIn), FieldTypes.Link)]
    [JsonSubtypes.KnownSubType(typeof(FieldAudioDataIn), FieldTypes.Audio)]
    public class FieldDataIn : IViewModeDataIn
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public List<string> Value { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public bool IsRequired { get; set; }
        public bool IsBold { get; set; }
        public FormHelpDataIn Help { get; set; }
        public bool IsHiddenOnPdf { get; set; }
        public bool IsReadOnlyViewMode { get; set; }
        public bool? AllowSaveWithoutValue { get; set; }
        public List<int> NullFlavors { get; set; } = new List<int>();
        public string FormId { get; set; }
        public string FieldSetId { get; set; }
        public DependentOnInfoDataIn DependentOn { get; set; }
        
    }
}