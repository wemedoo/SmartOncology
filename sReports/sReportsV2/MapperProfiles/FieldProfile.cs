using AutoMapper;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FieldEntity.Custom;
using sReportsV2.Domain.Entities.FieldEntity.Custom.Action;
using sReportsV2.DTOs.DTOs.Field.DataIn;
using sReportsV2.DTOs.DTOs.Field.DataIn.Custom;
using sReportsV2.DTOs.DTOs.Field.DataOut.Custom;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataIn.Custom;
using sReportsV2.DTOs.Field.DataIn.Custom.Action;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Field.DataOut.Custom;
using sReportsV2.DTOs.Field.DataOut.Custom.Action;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.Dependency;
using sReportsV2.Common.Extensions;

namespace sReportsV2.MapperProfiles
{
    public class FieldProfile : Profile
    {
        public FieldProfile()
        {
            CreateMap<Field, FieldDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FieldCalculative, FieldCalculativeDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldCheckbox, FieldCheckboxDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectable, FieldSelectableDataOut>();

            CreateMap<FieldDate, FieldDateDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldDatetime, FieldDatetimeDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldEmail, FieldEmailDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldFile, FieldFileDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldNumeric, FieldNumericDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldRadio, FieldRadioDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectable, FieldSelectableDataOut>();

            CreateMap<FieldRegex, FieldRegexDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldSelect, FieldSelectDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectable, FieldSelectableDataOut>();

            CreateMap<FieldTextArea, FieldTextAreaDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldText, FieldTextDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldRichTextParagraphDataIn, FieldRichTextParagraph>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldRichTextParagraph, FieldRichTextParagraphDataOut>()
                 .IgnoreAllNonExisting()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldString, FieldStringDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldSelectable, FieldSelectableDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldDataIn, Field>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<FormHelpDataIn, Help>()
                .ReverseMap();

            CreateMap<FieldCalculativeDataIn, FieldCalculative>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldCheckboxDataIn, FieldCheckbox>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectable>();

            CreateMap<FieldDateDataIn, FieldDate>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldDatetimeDataIn, FieldDatetime>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldEmailDataIn, FieldEmail>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldFileDataIn, FieldFile>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldNumericDataIn, FieldNumeric>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldRadioDataIn, FieldRadio>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectable>();

            CreateMap<FieldRegexDataIn, FieldRegex>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldSelectDataIn, FieldSelect>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectable>();

            CreateMap<FieldTextAreaDataIn, FieldTextArea>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldRichTextParagraphDataIn, FieldRichTextParagraph>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldTextDataIn, FieldText>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldStringDataIn, FieldString>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldSelectableDataIn, FieldSelectable>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldDataIn, FieldDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FieldCalculativeDataIn, FieldCalculativeDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldCheckboxDataIn, FieldCheckboxDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectableDataOut>();

            CreateMap<FieldDateDataIn, FieldDateDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldDatetimeDataIn, FieldDatetimeDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldEmailDataIn, FieldEmailDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldFileDataIn, FieldFileDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldNumericDataIn, FieldNumericDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldRadioDataIn, FieldRadioDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectableDataOut>();

            CreateMap<FieldRegexDataIn, FieldRegexDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldSelectDataIn, FieldSelectDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectableDataOut>();

            CreateMap<FieldTextAreaDataIn, FieldTextAreaDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldRichTextParagraphDataIn, FieldRichTextParagraphDataOut>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldTextDataIn, FieldTextDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldStringDataIn, FieldStringDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldSelectableDataIn, FieldSelectableDataOut>()
                .IgnoreAllNonExisting()
                 .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<CustomFieldButtonDataIn, CustomFieldButton>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, Field>();

            CreateMap<CustomFieldButtonDataIn, CustomFieldButtonDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<CustomFieldButton, CustomFieldButtonDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldCodedDataIn, FieldCoded>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldCodedDataIn, FieldCodedDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldCoded, FieldCodedDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldConnectedDataIn, FieldConnected>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldConnectedDataIn, FieldConnectedDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldConnected, FieldConnectedDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<CustomAction, CustomActionDataOut>();
            CreateMap<CustomActionDataIn, CustomActionDataOut>();
            CreateMap<CustomActionDataIn, CustomAction>();

            /*Javascript Action mapping*/
            CreateMap<JavascriptActionDataIn, JavascriptAction>()
                .IncludeBase<CustomActionDataIn, CustomAction>();

            CreateMap<JavascriptAction, JavascriptActionDataOut>()
                .IncludeBase<CustomAction, CustomActionDataOut>();

            CreateMap<JavascriptActionDataIn, JavascriptActionDataOut>()
                .IncludeBase<CustomActionDataIn, CustomActionDataOut>();

            /*Custom Action mapping*/
            CreateMap<ControllerActionDataIn, ControllerAction>()
                .IncludeBase<CustomActionDataIn, CustomAction>();

            CreateMap<ControllerAction, ControllerActionDataOut>()
                .IncludeBase<CustomAction, CustomActionDataOut>();

            CreateMap<ControllerActionDataIn, ControllerActionDataOut>()
                .IncludeBase<CustomActionDataIn, CustomActionDataOut>();

            CreateMap<CustomFieldFilterDataIn, CustomFieldFilterData>()
                .ReverseMap();

            CreateMap<CustomFieldFilterGroup, CustomFieldFilterDataOut>()
                .ForMember(d => d.CustomFieldFiltersId, dst => dst.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<FieldParagraphDataIn, FieldParagraph>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldParagraphDataIn, FieldParagraphDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldParagraph, FieldParagraphDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldLinkDataIn, FieldLink>()
                .IgnoreAllNonExisting()
                .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldLinkDataIn, FieldLinkDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldLink, FieldLinkDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldAudioDataIn, FieldAudio>()
                .IgnoreAllNonExisting()
            .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldAudioDataIn, FieldAudioDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldAudio, FieldAudioDataOut>()
                .IgnoreAllNonExisting()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<DependentOnInfoDataIn, DependentOnInfoDataOut>()
                .ReverseMap();
            CreateMap<DependentOnFieldInfoDataIn, DependentOnFieldInfoDataOut>()
                .ReverseMap();

            CreateMap<DependentOnInfoDataIn, DependentOnInfo>()
                .AfterMap((dataIn, entity) =>
                {
                    if (entity.FieldActions == null || entity.FieldActions.Count == 0)
                    {
                        entity.FieldActions = new System.Collections.Generic.List<Common.Enums.FieldAction> { Common.Enums.FieldAction.DataCleaning };
                    }
                });

            CreateMap<DependentOnInfo, DependentOnInfoDataIn>();
            CreateMap<DependentOnFieldInfoDataIn, DependentOnFieldInfo>()
                .ReverseMap();
            CreateMap<DependentOnInfo, DependentOnInfoDataOut>();
            CreateMap<DependentOnFieldInfo, DependentOnFieldInfoDataOut>();
        }
    }
}