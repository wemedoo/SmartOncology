using sReports.PathoLink.Entities;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.CodeEntry.DataOut;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.DTOs.User.DTO;
using System.Globalization;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PatholinkBLL : IPatholinkBLL
    {
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormDAL formDAL;
        private readonly ICodeAssociationDAL codeAssociationDAL;

        public PatholinkBLL(IFormInstanceDAL formInstanceDAL, IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL, IEncounterDAL encounterDAL, IFormDAL formDAL, ICodeAssociationDAL codeAssociationDAL)
        {
            this.formInstanceDAL = formInstanceDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.patientDAL = patientDAL;
            this.encounterDAL = encounterDAL;
            this.formDAL = formDAL;
            this.codeAssociationDAL = codeAssociationDAL;
        }

        public bool Import(PathoLink pathoLink, UserCookieData userCookieData)
        {
            FormInstance formInstance = formInstanceDAL.GetById(pathoLink.CaseDetails.submissionID);

            if (formInstance == null)
            {
                return false;
            }
            Form form = formDAL.GetForm(formInstance.FormDefinitionId);

            List<PathoLinkField> fields = pathoLink.ClinicalInformation.Concat<PathoLinkField>(pathoLink.Result).ToList();
            NormalizePathlinkFields(fields);

            foreach (FieldInstance fv in formInstance.FieldInstances)
            {
                switch (fv.Type)
                {
                    case FieldTypes.Calculative:
                    case FieldTypes.Date:
                    case FieldTypes.Datetime:
                    case FieldTypes.Digits:
                    case FieldTypes.Email:
                    case FieldTypes.LongText:
                    case FieldTypes.Number:
                    case FieldTypes.Regex:
                    case FieldTypes.Text:
                    case FieldTypes.URL:
                    case FieldTypes.Paragraph:
                    case FieldTypes.Link:
                    case FieldTypes.Audio:
                        fv.FieldInstanceValues = new List<FieldInstanceValue> {
                            GetStringFieldInstanceValueFromPathoLink(fields, fv.FieldId)
                        };
                        break;

                    case FieldTypes.Checkbox:
                    case FieldTypes.Select:
                    case FieldTypes.Radio:
                        fv.FieldInstanceValues = new List<FieldInstanceValue> {
                            GetSelectableFieldInstanceValueFromPathoLink(form.GetAllFields(), fields, fv)
                        };
                        break;
                }
            }

            formInstance.Id = null;
            formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));

            return true;
        }

        public async Task<PathoLink> Export(string formInstanceId, UserCookieData userCookieData)
        {
            Dictionary<int, Dictionary<int, string>> missingValues = codeAssociationDAL.InitializeMissingValueList(userCookieData.ActiveLanguage);

            FormInstance formInstance = formInstanceDAL.GetById(formInstanceId);
            Form populatedForm = new Form(formInstance, formDAL.GetForm(formInstance.FormDefinitionId))
            {
                Id = formInstance.Id
            };
            Patient patient = await GetPatient(formInstance).ConfigureAwait(false);

            List<PathoLinkField> dataOutFields = new List<PathoLinkField>();
            MapFieldsToPathoLink(populatedForm, dataOutFields, missingValues);

            PathoLink result = new PathoLink
            {
                CaseDetails = new CaseDetails()
                {
                    birthday = patient != null && patient.BirthDate.HasValue ? patient.BirthDate.Value.ToString(DateConstants.DateFormat, CultureInfo.InvariantCulture) : "",
                    dateOfSurgery = formInstance.Date.HasValue ? formInstance.Date.Value.ToString(DateConstants.DateFormat, CultureInfo.InvariantCulture) : "",
                    gender = GetPatholinkGender(patient?.GenderCD),
                    submissionID = formInstance.Id

                },
                ClinicalInformation = dataOutFields
            };

            return result;
        }

        private FieldInstanceValue GetSelectableFieldInstanceValueFromPathoLink(List<Field> fields, List<PathoLinkField> patholinkFields, FieldInstance fieldValue)
        {
            List<string> selectedOptionsIds = new List<string>();
            FieldSelectable fieldSelectable = fields.Find(x => x.Id.Equals(fieldValue.FieldId)) as FieldSelectable;

            foreach (FormFieldValue value in fieldSelectable.Values)
            {
                PathoLinkField patholinkField = patholinkFields.Find(x => x.o40MtId.Equals($"{fieldValue.FieldId}-{value.ThesaurusId}"));
                if (patholinkField.value == "true")
                {
                    selectedOptionsIds.Add(value.Id);
                }
            }

            return fieldSelectable.CreateFieldInstanceValue(selectedOptionsIds);
        }

        private FieldInstanceValue GetStringFieldInstanceValueFromPathoLink(List<PathoLinkField> fields, string id)
        {
            return new FieldInstanceValue(fields.Find(x => x.o40MtId.Equals(id))?.value);
        }

        private void NormalizePathlinkFields(List<PathoLinkField> fields)
        {
            foreach (PathoLinkField field in fields)
            {
                field.RemoveFormIdFromO4MTId();
            }
        }

        private string GetPatholinkGender(int? genderCodeId)
        {
            string patholinkGender = string.Empty;
            CodeDataOut genderCode = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender).Find(c => c.Id == genderCodeId);

            if (genderCode != null)
            {
                string genderName = genderCode.Thesaurus.GetPreferredTermByTranslationOrDefault(LanguageConstants.EN);
                if (genderName == Gender.Male.ToString())
                {
                    patholinkGender = "M";
                }
                else if (genderName == Gender.Female.ToString())
                {
                    patholinkGender = "F";
                }
            }

            return patholinkGender;
        }

        private async Task<Patient> GetPatient(FormInstance formInstance)
        {
            Encounter encounter = await encounterDAL.GetByIdAsync(formInstance.EncounterRef).ConfigureAwait(false);
            if (encounter == null) return null;
            EpisodeOfCare episodeOfCareEntity = episodeOfCareDAL.GetById(encounter.EpisodeOfCareId);

            return patientDAL.GetById(episodeOfCareEntity.PatientId);
        }

        private void MapFieldsToPathoLink(Form populatedForm, List<PathoLinkField> dataOutFields, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            List<Field> fields = populatedForm.GetAllFields();
            Dictionary<string, Field> dictionaryFields = fields.ToDictionary(f => f.Id, f => f);
            foreach (Field field in fields)
            {
                ParseToPathoLinkField(populatedForm.Id, dictionaryFields, field, dataOutFields, missingValues);
            }
        }

        private void ParseToPathoLinkField(string formInstanceId, Dictionary<string, Field> dictionaryFields, Field field, List<PathoLinkField> dataOutFields, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            if (field is FieldSelectable selectable)
            {
                InsertSelectableField(formInstanceId, dictionaryFields, selectable, dataOutFields, missingValues);
            }
            else
            {
                InsertNonSelectableField(formInstanceId, dictionaryFields, field, dataOutFields, missingValues);
            }
        }

        private void InsertNonSelectableField(string formInstanceId, Dictionary<string, Field> dictionaryFields, Field field, List<PathoLinkField> dataOutFields, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            string o40mtid = $"{formInstanceId}-{field.Id}";
            string exportPatholinkValue = field.GetValueForPatholinkExport(missingValues);

            PathoLinkField pathLinkField = new PathoLinkField
            {
                value = exportPatholinkValue,
                name = $"[{field.Label}] [{field.Label}]{field.GenerateDependentSuffix(dictionaryFields)}",
                defaultValue = string.Empty,
                type = field.Type,
                o40MtId = o40mtid
            };

            AddPatholinkField(pathLinkField, dataOutFields, o40mtid);
        }

        private void InsertSelectableField(string formInstanceId, Dictionary<string, Field> dictionaryFields, FieldSelectable field, List<PathoLinkField> dataOutFields, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            field.Values.ForEach(x =>
            {
                string name = $"[{field.Label}] [{x.Value}]{field.GenerateDependentSuffix(dictionaryFields)}";
                string o40mtid = $"{formInstanceId}-{field.Id}-{x.ThesaurusId}";
                string exportPatholinkValue = field.GetValueForPatholinkExport(missingValues, x.Id);

                PathoLinkField pathLinkField = new PathoLinkField
                {
                    value = exportPatholinkValue,
                    name = name,
                    defaultValue = "false",
                    type = field.Type,
                    o40MtId = o40mtid
                };

                AddPatholinkField(pathLinkField, dataOutFields, o40mtid);
            });
        }

        private void AddPatholinkField(PathoLinkField pathLinkField, List<PathoLinkField> addedPatholinkFields, string o40mtid)
        {
            if (!addedPatholinkFields.Exists(y => y.o40MtId.Equals(o40mtid)))
            {
                addedPatholinkFields.Add(pathLinkField);
            }
        }
    }
}
