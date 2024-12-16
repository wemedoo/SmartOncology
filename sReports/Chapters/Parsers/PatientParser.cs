using Chapters.Resources;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.Cache.Singleton;

namespace Chapters
{
    public class PatientParser
    {
        private readonly List<string> identifierList;
        private FormChapter patientChapter;
        private List<CodeDataOut> countrieCodes;
        private List<CodeDataOut> telecomUseCodes;
        private List<CodeDataOut> telecomSystemCodes;
        private List<CodeDataOut> genderCodes;
        private List<CodeDataOut> contactRelationshipCodes;
        private List<CodeDataOut> identifierTypeCodes;
        private List<CodeDataOut> identifierUseTypeCodes;
        private List<CodeDataOut> languageCodes;

        public PatientParser()
        {
            this.identifierList = Enum.GetNames(typeof(TypeOfIdentifier)).ToList();
            SetCodes();
        }

        public Patient ParsePatientChapter(FormChapter chapter)
        {
            patientChapter = chapter;
            Patient result = ParseIntoPatient();            
            return result;
        }

        private void SetCodes()
        {
            this.countrieCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Country);
            this.telecomSystemCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TelecomSystemType);
            this.telecomUseCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.TelecomUseType);
            this.genderCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Gender);
            this.contactRelationshipCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ContactRelationship);
            this.identifierTypeCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.IdentifierUseType);
            this.identifierUseTypeCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.IdentifierUseType);
            this.languageCodes = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.Language);
        }

        private Patient ParseIntoPatient()
        {
            Patient patient = null;
            if (patientChapter != null)
            {
                List<Field> basicInfoFields = patientChapter.GetFieldsByList(PatientRelatedLists.BasicInfoList.ToList());
                List<Field> identifiersFields = patientChapter.GetFieldsByList(identifierList);
                List<Field> addressInfoFields = patientChapter.GetFieldsByList(PatientRelatedLists.AddressInfoList.ToList());
                List<Field> contactPersonFields = patientChapter.GetFieldsByList(PatientRelatedLists.ContactPersonList.ToList());
                List<Field> telecomFields = patientChapter.GetFieldsByList(PatientRelatedLists.TelecomValues.ToList());

                patient = ParseBasicIntoPatient(basicInfoFields);
                patient.PatientTelecoms = GetTelecomsForPatient(telecomFields, patient.PatientId);
                patient.PatientIdentifiers = GetListIdentifiers(identifiersFields);
                patient.PatientAddresses =
                    new List<PatientAddress>() { new PatientAddress(ParseIntoPatientAddress(addressInfoFields), patient.PatientId) }
                    ;
                patient.PatientContacts = new List<PatientContact> { ParseIntoContactPerson(contactPersonFields) };
            }

            return patient;
        }

        public List<PatientIdentifier> GetListIdentifiers(List<Field> identifiersFields)
        {
            List<PatientIdentifier> identifiers = new List<PatientIdentifier>();
            PatientIdentifier identifier = ParseIntoIdentifier(identifiersFields);
            if (identifier != null)
            {
                identifiers.Add(identifier);
            }

            return identifiers;
        }

        private Patient ParseBasicIntoPatient(List<Field> basicInfoList)
        {
            Patient patientEntity = new Patient(
                GetFieldValue(basicInfoList, PdfParserType.Name, true),
                GetFieldValue(basicInfoList, PdfParserType.Family, true)
                );

            var patientField = basicInfoList.Find(x => x.FhirType ==  PdfParserType.BirthDate);

            if (!string.IsNullOrEmpty(patientField?.FieldInstanceValues.GetFirstValue()))
            {
                patientEntity.BirthDate = Convert.ToDateTime(patientField.FieldInstanceValues.GetFirstValue());
            }

            patientEntity.Communications = new List<Communication>();
            Communication communication = new Communication
            {
                LanguageCD = GetCodeId(languageCodes, basicInfoList.Find(x => x.FhirType == PdfParserType.Language)?.FieldInstanceValues?.GetFirstValue()),
                Preferred = true
            };

            if (communication.LanguageCD.HasValue)
            {
                patientEntity.Communications.Add(communication);
            }

            patientEntity.GenderCD = GetCodeId(genderCodes, basicInfoList.Find(x => x.FhirType == PdfParserType.Gender)?.FieldInstanceValues?.GetFirstValue());

            return patientEntity;
        }

        private PatientIdentifier ParseIntoIdentifier(List<Field> identifierFields)
        {
            PatientIdentifier identifier = null;
            int? identifierTypeCodeId = GetCodeId(identifierTypeCodes, identifierFields.Find(x => x.FhirType == PdfParserType.IdentifierName)?.FieldInstanceValues?.GetFirstValue());
            int? useTypeCodeId = GetCodeId(identifierUseTypeCodes, identifierFields.Find(x => x.FhirType == PdfParserType.IdentifierUse)?.FieldInstanceValues?.GetFirstValue());
            string value = identifierFields.Find(x => x.FhirType == PdfParserType.IdentifierValue)?.FieldInstanceValues?.GetFirstValue();

            if (identifierTypeCodeId.HasValue && !string.IsNullOrWhiteSpace(value))
            {
                identifier = new PatientIdentifier(identifierTypeCodeId, value, useTypeCodeId);
            }

            return identifier;
        }

        private AddressBase ParseIntoPatientAddress(List<Field> addressFields)
        {
            AddressBase address = new PatientAddress
            {
                City = addressFields.Find(x => x.FhirType == PdfParserType.City)?.FieldInstanceValues?.GetFirstValue(),
                State = addressFields.Find(x => x.FhirType == PdfParserType.State)?.FieldInstanceValues?.GetFirstValue(),
                PostalCode = addressFields.Find(x => x.FhirType == PdfParserType.PostalCode)?.FieldInstanceValues?.GetFirstValue(),
                CountryCD = GetCountryId(addressFields, PdfParserType.Country),
                Street = addressFields.Find(x => x.FhirType == PdfParserType.Street)?.FieldInstanceValues?.GetFirstValue()
            };

            return address;
        }

        private List<PatientContactTelecom> GetTelecomsForContact(List<Field> contactFields)
        {
            List<PatientContactTelecom> telecoms = new List<PatientContactTelecom>();
            SetTelecom(nameof(TelecomSystemType.Phone), PdfParserType.ContactPhone, PdfParserType.ContactPhoneUse, contactFields, telecoms);
            SetTelecom(nameof(TelecomSystemType.Email), PdfParserType.ContactEmail, PdfParserType.ContactEmailUse, contactFields, telecoms);
            SetTelecom(nameof(TelecomSystemType.Url), PdfParserType.ContactUrl, PdfParserType.ContactUrlUse, contactFields, telecoms);
            SetTelecom(nameof(TelecomSystemType.Fax), PdfParserType.ContactFax, PdfParserType.ContactFaxUse, contactFields, telecoms);
            SetTelecom(nameof(TelecomSystemType.Sms), PdfParserType.ContactSms, PdfParserType.ContactSmsUse, contactFields, telecoms);
            SetTelecom(nameof(TelecomSystemType.Other), PdfParserType.ContactOther, PdfParserType.ContactOtherUse, contactFields, telecoms);
            SetTelecom(nameof(TelecomSystemType.Pager), PdfParserType.ContactPager, PdfParserType.ContactPagerUse, contactFields, telecoms);

            return telecoms;
        }

        private List<PatientTelecom> GetTelecomsForPatient(List<Field> telecomsOptions, int patientId)
        {
            List<PatientTelecom> telecoms = new List<PatientTelecom>();
            SetTelecom(nameof(TelecomSystemType.Phone), PdfParserType.Phone, PdfParserType.PhoneUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Email), PdfParserType.Email, PdfParserType.EmailUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Url), PdfParserType.Url, PdfParserType.UrlUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Fax), PdfParserType.Fax, PdfParserType.FaxUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Sms), PdfParserType.Sms, PdfParserType.SmsUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Other), PdfParserType.Other, PdfParserType.OtherUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Pager), PdfParserType.Pager, PdfParserType.PagerUse, telecomsOptions, telecoms);

            return telecoms.Select(telecom => new PatientTelecom(telecom, patientId)).ToList();
        }


        private void SetTelecom<T>(string system, string value, string use, List<Field> telecomsOptions, List<T> telecoms) where T : TelecomBase, new()
        {
            var phoneUse = telecomsOptions.Find(x => x.FhirType == use)?.FieldInstanceValues?.GetFirstValue();
            var phone = telecomsOptions.Find(x => x.FhirType == value)?.FieldInstanceValues?.GetFirstValue();

            if (!string.IsNullOrEmpty(phoneUse) && !string.IsNullOrEmpty(phone))
            {
                int? systemCD = GetCodeId(telecomSystemCodes, system);
                int? useCD = GetCodeId(telecomUseCodes, use);
                telecoms.Add(new T() 
                    {
                        SystemCD = systemCD,
                        UseCD = useCD,
                        Value = value
                    }
                );
            }
        }


        private PatientContact ParseIntoContactPerson(List<Field> contactFields)
        {
            PatientContactAddress contactAddress = new PatientContactAddress
            {
                City = contactFields.Find(x => x.FhirType == PdfParserType.ContactCity)?.FieldInstanceValues?.GetFirstValue(),
                State = contactFields.Find(x => x.FhirType == PdfParserType.ContactState)?.FieldInstanceValues?.GetFirstValue(),
                PostalCode = contactFields.Find(x => x.FhirType == PdfParserType.ContactPostalCode)?.FieldInstanceValues?.GetFirstValue(),
                CountryCD = GetCountryId(contactFields, PdfParserType.ContactCountry),
                Street = contactFields.Find(x => x.FhirType == PdfParserType.ContactStreet)?.FieldInstanceValues?.GetFirstValue()
            };

            PatientContact contactPerson = new PatientContact
            {
                PatientContactAddresses = new List<PatientContactAddress>() {contactAddress},
                NameGiven = GetFieldValue(contactFields, PdfParserType.ContactName, true),
                NameFamily = GetFieldValue(contactFields, PdfParserType.ContactFamily, true),
                PatientContactTelecoms = new List<PatientContactTelecom>()
            };

            List<Field> telecomFields = contactFields.Where(x => PatientRelatedLists.ContactTelecomValues.Contains(x.FhirType)).ToList();
            contactPerson.PatientContactTelecoms = GetTelecomsForContact(telecomFields);
            contactPerson.GenderCD = GetCodeId(genderCodes, contactFields.Find(x => x.FhirType == PdfParserType.ContactGender)?.FieldInstanceValues?.GetFirstValue());
            contactPerson.ContactRelationshipCD = GetCodeId(contactRelationshipCodes, contactFields.Find(x => x.FhirType == PdfParserType.Relationship)?.FieldInstanceValues?.GetFirstValue());

            return contactPerson;
        }

        private string GetFieldValue(List<Field> fields, string fhirType, bool setDefaultValue = false)
        {
            string fieldValue = fields?.Find(x => x.FhirType == fhirType)?.FieldInstanceValues?.GetFirstValue();
            if (string.IsNullOrEmpty(fieldValue) && setDefaultValue)
            {
                fieldValue = PdfParserType.Unknown;
            }

            return fieldValue;
        }

        private int? GetCountryId(List<Field> addressFields, string fhirType)
        {
            string countryName = addressFields.Find(x => x.FhirType == fhirType)?.FieldInstanceValues?.GetFirstValue();
            int? countryCD = null;
            if (!string.IsNullOrEmpty(countryName))
            {
                countryCD = countrieCodes.Where(e => e.Thesaurus.Translations.Exists(t => t.PreferredTerm == countryName)).Select(x => x.Id).FirstOrDefault();
            }

            return countryCD;
        }

        private int? GetCodeId(List<CodeDataOut> codes, string codeName)
        {
            int? codeId = codes?.Find(e => e.Thesaurus.Translations.Exists(t => t.PreferredTerm == codeName))?.Id;

            return codeId;
        }
    }
}
