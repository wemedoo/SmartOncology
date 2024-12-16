using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Newtonsoft.Json;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FormInstance;
using System.Globalization;

namespace Chapters
{
    public class PdfFormParser
    {
        public Patient Patient { get; set; }
        private readonly PdfDocument pdfDocument;
        private PdfAcroForm pdfAcroForm;
        private readonly Form formJson;
        private List<FieldInstance> fieldInstances = new List<FieldInstance>();

        public PdfFormParser(string jsonFormPath)
        {
            formJson = JsonConvert.DeserializeObject<Form>(File.ReadAllText(jsonFormPath));
        }

        public PdfFormParser(Form form, PdfDocument document)
        {
            this.pdfDocument = document;
            this.formJson = form;
        }

        public List<FieldInstance> FieldInstances
        {
            get => fieldInstances;
            set => fieldInstances = value ?? new List<FieldInstance>();
        }

        public Form ReadFieldsFromPdf()
        {
            pdfAcroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);

            var allPdfKeys = pdfAcroForm.GetFormFields().Select(x => x.Key);
            foreach (List<FieldSet> list in formJson.GetAllListOfFieldSets())
            {
                int fieldSetCount = GetFieldSetCount(allPdfKeys, list[0].Id);
                AddRepetititveFieldSets(list, fieldSetCount);
            }

            ParseFields();
            ParsePatient();

            // There are no side effects with this code
            //if (Patient != null)
            //{
            //    List<CodeDataOut> telecomSystemCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TelecomSystemType);
            //    InsertTelecomCheckboxValue(PrepareTelecomSystems(Patient.PatientTelecoms.Select(t => t.SystemCD), telecomSystemCodes), "TelecomCheckBox");
            //    InsertTelecomCheckboxValue(PrepareTelecomSystems(Patient.PatientContacts.SelectMany(c => c.PatientContactTelecoms.Select(t => t.SystemCD)), telecomSystemCodes), "ContactTelecomCheckBox");
            //}

            formJson.FormState = sReportsV2.Common.Enums.FormState.Finished;
            return formJson;
        }

        private int GetFieldSetCount(IEnumerable<string> allPdfKeys, string fieldSetId)
        {
            var allKeysForFieldSetId = allPdfKeys.Where(x => x.Split('-')[0].Equals(fieldSetId));
            List<string> fieldSetIdentificators = new List<string>();
            foreach (var key in allKeysForFieldSetId)
            {
                fieldSetIdentificators.Add(GetFieldSetPosition(key));
            }

            return fieldSetIdentificators.Distinct().Count();
        }

        private void AddRepetititveFieldSets(List<FieldSet> list, int count) 
        {
            if (list.Count == 1)
            {
                FieldSet fieldSet = list.First();
                fieldSet.FieldSetInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();

                for (int i = 1; i < count; i++)
                {
                    FieldSet repetitiveFieldSet = fieldSet.Clone();
                    fieldSet.FieldSetInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
                    list.Add(repetitiveFieldSet);
                }
            } 
        }

        private void ParseFields()
        {
            foreach (KeyValuePair<string, PdfFormField> fieldFromPdf in pdfAcroForm.GetFormFields())
            {
                string key = fieldFromPdf.Key;
                string[] partsOfKey = key.Split('-');
                if (partsOfKey.Count() > 1)
                {
                    (FieldSet fieldSet, int fieldSetPosition) = GetFieldSet(partsOfKey);
                    Field field = fieldSet.Fields.Find(x => x.Id.Equals(partsOfKey[1]));

                    RemoveIfExist(fieldInstances, fieldSet.Id, fieldSetPosition, field?.Id);
                    fieldInstances.Add(new FieldInstance(
                        field, 
                        fieldSet.Id, 
                        fieldSet.FieldSetInstanceRepetitionId,
                        ParseFieldInstanceValue(field, partsOfKey, fieldFromPdf.Value)
                        )
                    {
                        FieldSetCounter = fieldSetPosition
                    });
                }
                else 
                {
                    SetNoteAndDate(key, fieldFromPdf.Value);
                }
            }
        }

        private (FieldSet, int) GetFieldSet(string[] partsOfKey)
        {
            string fieldSetId = partsOfKey[0];
            List<FieldSet> listFieldSet = formJson.GetListOfFieldSetsByFieldSetId(fieldSetId);
            int fieldSetPosition = Int32.Parse(GetFieldSetPosition(partsOfKey));
            return (listFieldSet[fieldSetPosition], fieldSetPosition);
        }

        private FieldInstanceValue ParseFieldInstanceValue(Field field, string[] partsOfKey, PdfFormField fieldFromPdf)
        {
            switch (field)
            {
                case FieldRadio fieldRadio:
                    return ParseSelectableFieldInstanceValue(fieldRadio, GetValueRadio(fieldRadio, fieldFromPdf));
                case FieldCheckbox fieldCheckbox:
                    return ParseSelectableFieldInstanceValue(fieldCheckbox, GetValueCheckbox(field.Id, fieldCheckbox.Values, GetFieldSetPosition(partsOfKey)));
                case FieldSelect fieldSelect:
                    return ParseSelectableFieldInstanceValue(fieldSelect, GetValueSelect(fieldSelect, fieldFromPdf));
                case FieldCalculative fieldCalculative:
                    return new FieldInstanceValue(fieldCalculative.GetCalculation(GetFieldValuesForFormula(partsOfKey, fieldCalculative)));
                default:
                    return new FieldInstanceValue(fieldFromPdf.GetValueAsString());
            }
        }

        private FieldInstanceValue ParseSelectableFieldInstanceValue(FieldSelectable fieldSelectable, IEnumerable<string> selectedOptionsIds)
        {
            return fieldSelectable.CreateFieldInstanceValue(selectedOptionsIds.ToList());
        }

        private IEnumerable<string> GetValueRadio(FieldRadio fieldRadio, PdfFormField fieldFromPdf)
        {
            return fieldRadio.Values.Where(x => x.Id == fieldFromPdf.GetValueAsString()).Select(x => x.Id);
        }

        private IEnumerable<string> GetValueSelect(FieldSelect fieldRadio, PdfFormField fieldFromPdf)
        {
            return fieldRadio.Values.Where(x => x.Label == fieldFromPdf.GetValueAsString()).Select(x => x.Id);
        }

        private List<string> GetValueCheckbox(string formFieldId, List<FormFieldValue> formFieldValues, string fieldSetPosition)
        {
            List<string> selectedFormFieldValuesIds = new List<string>();
            foreach (var field in pdfAcroForm.GetFormFields())
            {
                if (field.Value.GetValue() != null 
                    && field.Key.Split('-').Count() > 1 
                    && formFieldId == GetFieldId(field.Key)
                    && fieldSetPosition == GetFieldSetPosition(field.Key)
                    && field.Value.GetValue().ToString() == "/Yes")
                {
                    selectedFormFieldValuesIds.Add(formFieldValues.Find(x => x.Id == GetCheckBoxOptionId(field.Key))?.Id);
                }
            }

            return selectedFormFieldValuesIds;
        }

        private string GetFieldSetPosition(string key)
        {
            return GetFieldSetPosition(key.Split('-'));
        }

        private string GetFieldSetPosition(string[] keySplitted)
        {
            return keySplitted.Last().Split('.')[0];
        }

        private string GetFieldId(string key)
        {
            string[] splitedKey = key.Split('-');

            return splitedKey[1];
        }

        private string GetCheckBoxOptionId(string key)
        {
            string[] splitedKey = key.Split('-');

            return splitedKey[4];
        }

        private Dictionary<string, string> GetFieldValuesForFormula(string[] partsOfKey, FieldCalculative fieldCalculative)
        {
            (FieldSet fieldSet, _) = GetFieldSet(partsOfKey);

            Dictionary<string, string> fieldValuesForFormula = new Dictionary<string, string>();
            foreach (string fieldId in fieldCalculative.IdentifiersAndVariables.Keys)
            {
                string fieldValue = GetFieldInstance(fieldSet, fieldId)?.FieldInstanceValues?.GetFirstValue();
                fieldValuesForFormula.Add(fieldId, fieldValue);
            }

            return fieldValuesForFormula;
        }

        private FieldInstance GetFieldInstance(FieldSet fieldSet, string fieldId)
        {
            if (fieldSet.IsRepetitive)
            {
                return fieldInstances.Find(x => 
                    x.FieldSetInstanceRepetitionId == fieldSet.FieldSetInstanceRepetitionId 
                    && x.FieldId.Equals(fieldId)
                    );
            }
            else
            {
                return fieldInstances.Find(x => 
                    x.FieldId.Equals(fieldId)
                    );
            }
        }

        private void RemoveIfExist(List<FieldInstance> fieldInstances, string fieldSetId, int fieldSetPosition, string fieldId)
        {
            FieldInstance fieldValue = fieldInstances.Find(x => x.FieldSetId == fieldSetId && x.FieldId == fieldId && x.FieldSetCounter == fieldSetPosition);
            if (fieldValue != null)
            {
                fieldInstances.Remove(fieldValue);
            }
        }

        private void SetNoteAndDate(string key, PdfFormField fieldFromPdf) 
        {
            string valueFromPdf = fieldFromPdf.GetValueAsString();
            if (key == "note")
            {
                formJson.Notes = valueFromPdf;
            }
            if (key == "date")
            {
                if (!string.IsNullOrWhiteSpace(valueFromPdf))
                {
                    formJson.Date = DateTime.Parse(valueFromPdf, CultureInfo.InvariantCulture).ToUniversalTime();
                }
                else 
                {
                    formJson.Date = DateTime.Now;
                }
            }
        }

        private void ParsePatient()
        {
            PatientParser patientParser = new PatientParser();
            Patient = patientParser.ParsePatientChapter(formJson.Chapters.Find(x => x.ThesaurusId.ToString().Equals(ResourceTypes.PatientThesaurus)));
        }

        // There are no side effects with this code
        //private void InsertTelecomCheckboxValue(IEnumerable<string> telecomSystems, string checkboxType)
        //{
        //    string value = string.Empty;
        //    List<string> values = new List<string>();
        //    Field formField = formJson.GetAllFields().Find(x => x.FhirType != null && x.FhirType.Equals(checkboxType));

        //    foreach (var telecomSystem in telecomSystems)
        //    {
        //        values.Add(telecomSystem);
        //    }

        //    formField.AddValue(formField != null ? string.Join(",", values) : "");

        //}

        //private IEnumerable<string> PrepareTelecomSystems(IEnumerable<int?> telecomSystemCodeIds, List<CodeDataOut> telecomSystemCodes)
        //{
        //    List<string> telecomSystems = new List<string>();
        //    foreach (int? telecomSystemCodeId in telecomSystemCodeIds)
        //    {
        //        CodeDataOut telecomSystemCode = telecomSystemCodes.Find(c => c.Id == telecomSystemCodeId);
        //        if (telecomSystemCode != null)
        //        {
        //            telecomSystems.Add(telecomSystemCode.Thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN));
        //        }
        //    }

        //    return telecomSystems;
        //}
    }
}


