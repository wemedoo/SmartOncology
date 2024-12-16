using Hl7.Fhir.Model;
using Hl7.FhirPath.Sprache;
using sReportsV2.BusinessLayer.Components.Implementations;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using sReportsV2.DTOs.Field.DataIn;

namespace sReportsV2.BusinessLayer.Helpers
{
    public class QuestionnaireExportHelper
    {
        private readonly string _thesaurusUrlTemplate;
        private readonly FormDataOut _form;
        private readonly List<FieldDataOut> _fields;
        private readonly string _host;
        private readonly IMapper _mapper;


        public QuestionnaireExportHelper(FormDataOut formDataOut, string host, IMapper Mapper)
        {
            _form = formDataOut;
            _fields = formDataOut.GetAllFields();
            _host = host;
            _thesaurusUrlTemplate = host.GetPreviewThesaurusUrlTemplate();
            _mapper = Mapper;
        }

        public Questionnaire CreateQuestionnaire()
        {
            Questionnaire questionnaire = new Questionnaire();
            SetBasicInfo(questionnaire);
            SetSubElements(questionnaire);

            return questionnaire;
        }

        private void SetBasicInfo(Questionnaire questionnaire)
        {
            questionnaire.Title = _form.Title;
            questionnaire.Description = new Markdown(_form.DocumentProperties?.Description);
            questionnaire.Status = GetQuestionnaireStatus(_form.State);
            questionnaire.Purpose = new Markdown(_form.Title);
            questionnaire.SubjectType = new List<ResourceType?> { ResourceType.Patient };
            questionnaire.Publisher = ResourceTypes.CompanyName;
            questionnaire.Copyright = new Markdown(ResourceTypes.CompanyCopyright);
            questionnaire.Url = $"{_host}Fhir/ExportFormToQuestionnaire?formId={_form.Id}&title={_form.Title}";
            questionnaire.Contact = new List<ContactDetail> {
                new ContactDetail { Name = $"{ResourceTypes.CompanyWebsite}/contact-us" } 
            };
        }

        private PublicationStatus GetQuestionnaireStatus(FormDefinitionState formDefinitionState)
        {
            switch (formDefinitionState)
            {
                case FormDefinitionState.ReadyForDataCapture: return PublicationStatus.Active;
                case FormDefinitionState.Archive: return PublicationStatus.Retired;
                case FormDefinitionState.Design:
                case FormDefinitionState.DesignPending:
                case FormDefinitionState.Review:
                case FormDefinitionState.ReviewPending:
                default: return PublicationStatus.Draft;
            }
        }

        private void SetSubElements(Questionnaire questionnaire)
        {
            foreach (FormChapterDataOut chapter in _form.Chapters)
            {
                Questionnaire.ItemComponent chapterItem = GetItem(chapter);
                foreach (FormPageDataOut page in chapter.Pages)
                {
                    Questionnaire.ItemComponent pageItem = GetItem(page);
                    foreach (FormFieldSetDataOut fieldSet in page.ListOfFieldSets.SelectMany(lFs => lFs))
                    {
                        Questionnaire.ItemComponent fieldSetItem = GetItem(fieldSet);
                        foreach (FieldDataOut field in fieldSet.Fields)
                        {
                            Questionnaire.ItemComponent fieldItem = GetItem(field);
                            fieldSetItem.Item.Add(fieldItem);
                        }
                        pageItem.Item.Add(fieldSetItem);
                    }
                    chapterItem.Item.Add(pageItem);
                }
                questionnaire.Item.Add(chapterItem);
            }
        }


        private Questionnaire.ItemComponent GetItem(string text, string linkId, Questionnaire.QuestionnaireItemType type, int thesaurusId, bool required=false)
        {
            return new Questionnaire.ItemComponent()
            {
                Text = text,
                LinkId = linkId,
                Type = type,
                Required = required,
                Code = new List<Coding>
                {
                    CreateCode(thesaurusId.ToString(), text)
                }
            };
        }

        public Questionnaire.ItemComponent GetItem(FormChapterDataOut chapter) {
            return GetItem(chapter.Title, chapter.Id, Questionnaire.QuestionnaireItemType.Group, chapter.ThesaurusId);
        }

        public Questionnaire.ItemComponent GetItem(FormPageDataOut page) {
            return GetItem(page.Title, page.Id, Questionnaire.QuestionnaireItemType.Group, page.ThesaurusId);
        }

        public Questionnaire.ItemComponent GetItem(FormFieldSetDataOut fieldset) {
            return GetItem(fieldset.Label, fieldset.Id, Questionnaire.QuestionnaireItemType.Group, fieldset.ThesaurusId);
        }

        public Questionnaire.ItemComponent GetItem(FieldDataOut field)
        {
            Questionnaire.ItemComponent item = GetItem(GetItemText(field), field.Id, GetItemType(field), field.ThesaurusId, field.IsRequired);
            SetPropertiesForField(item, field);
            return item;
        }

        private Questionnaire.QuestionnaireItemType GetItemType(FieldDataOut field)
        {
            switch (field.Type)
            {
                case FieldTypes.Number:
                    return GetNumericItemType(field as FieldNumericDataOut);
                case FieldTypes.Date:
                    return Questionnaire.QuestionnaireItemType.Date;
                case FieldTypes.Datetime:
                    return Questionnaire.QuestionnaireItemType.DateTime;
                case FieldTypes.Checkbox:
                case FieldTypes.Select:
                case FieldTypes.Radio:
                    return Questionnaire.QuestionnaireItemType.Choice;
                case FieldTypes.Paragraph:
                case FieldTypes.Link:
                    return Questionnaire.QuestionnaireItemType.Display;
                default:
                    return Questionnaire.QuestionnaireItemType.Text;
            }
        }

        private string GetItemText(FieldDataOut field)
        {
            switch (field.Type)
            {
                case FieldTypes.Paragraph:
                    {
                        string paragraphText = string.Empty;
                        if (!string.IsNullOrWhiteSpace(field.GetLabel()))
                        {
                            paragraphText = field.GetLabel().ExtractTextFromHtml().Trim();
                        }
                        return paragraphText;
                    }
                case FieldTypes.Link:
                    return (field as FieldLinkDataOut).Link;
                default:
                    return field.Label;
            }
        }

        private Questionnaire.QuestionnaireItemType GetNumericItemType(FieldNumericDataOut field)
        {
            Questionnaire.QuestionnaireItemType numericItemType = Questionnaire.QuestionnaireItemType.Decimal;
            if (field.Step.HasValue)
            {
                bool isInteger = field.Step.Value % 1 == 0;
                if (isInteger)
                {
                    numericItemType = Questionnaire.QuestionnaireItemType.Integer;
                }
            }
            return numericItemType;
        }

        private void SetPropertiesForField(Questionnaire.ItemComponent item, FieldDataOut field)
        {
            if (field is FieldSelectableDataOut fieldSelectable)
            {
                SetExtensions(item, field);
                SetAnswerOptionsIfNecessarry(item, fieldSelectable);
            }
            item.Repeats = field.IsFieldRepetitive;
            SetDependency(item, field);
        }

        private void SetAnswerOptionsIfNecessarry(Questionnaire.ItemComponent item, FieldSelectableDataOut fieldSelectable)
        {
            foreach (FormFieldValueDataOut option in fieldSelectable.Values)
            {
                Questionnaire.AnswerOptionComponent answerOptionComponent = new Questionnaire.AnswerOptionComponent()
                {
                    Value = CreateCode(option.ThesaurusId.ToString(), option.Label) 
                };
                AddNewAnswerOption(item, answerOptionComponent);
            }
        }

        private void AddNewAnswerOption(Questionnaire.ItemComponent item, Questionnaire.AnswerOptionComponent newAnswerOption)
        {
            if (item.AnswerOption == null)
            {
                item.AnswerOption = new List<Questionnaire.AnswerOptionComponent>();
            }
            item.AnswerOption.Add(newAnswerOption);
        }

        private void SetExtensions(Questionnaire.ItemComponent item, FieldDataOut field)
        {
            CodeableConcept codeableConcept = new CodeableConcept("http://hl7.org/fhir/ValueSet/questionnaire-item-control", GetQuestionnaireItemControlCode(field.Type));
            item.AddExtension("http://hl7.org/fhir/StructureDefinition/questionnaire-itemControl", codeableConcept);
        }

        private string GetQuestionnaireItemControlCode(string fieldType)
        {
            switch (fieldType)
            {
                case FieldTypes.Checkbox:
                    return "check-box";
                case FieldTypes.Select:
                    return "drop-down";
                case FieldTypes.Radio:
                    return "radio-button";
                default:
                    return "text";
            }
        }

        private void SetDependency(Questionnaire.ItemComponent item, FieldDataOut field)
        {
            field.AddMissingPropertiesInDependency(_form);
            DependentOnInfoDataOut dependentOn = field.DependentOn;
            if (dependentOn != null && !string.IsNullOrEmpty(dependentOn.Formula))
            {
                LogicalExpressionParser interpreter = new LogicalExpressionParser(_mapper.Map<DependentOnInfoDataIn>(dependentOn));
                LogicalExpression node = interpreter.Parse();

                LogicalExpressionFHIRAnalyzer fHIRAnalyzer = new LogicalExpressionFHIRAnalyzer(_fields, dependentOn, _host);
                List<Questionnaire.EnableWhenComponent> enable = (List<Questionnaire.EnableWhenComponent>)node.Accept(fHIRAnalyzer);
                item.EnableWhen.AddRange(enable);
                item.EnableBehavior = fHIRAnalyzer.GetLogicalOperator();
            }
        }

        private Coding CreateCode(string code, string display)
        {
            return new Coding(_thesaurusUrlTemplate + code, code, display);
        }
    }
}
