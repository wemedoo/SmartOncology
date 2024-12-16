using NHapi.Model.V231.Datatype;
using NHapi.Model.V231.Segment;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.Base;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.DTOs.PDF.DataOut;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using sReportsV2.HL7.DTOs;
using sReportsV2.HL7.Constants;

namespace sReportsV2.HL7
{
    public static class HL7OutgoingHelper
    {
        public static void ConfigureSegment(MSH mshSegment, string receivingApplication, string sendingFacility, HL7MessageTypeMetadataDTO messageTypeMetadata)
        {
            mshSegment.SendingApplication.NamespaceID.Value = ResourceTypes.SmartOncology;
            mshSegment.SendingFacility.NamespaceID.Value = sendingFacility;
            mshSegment.ReceivingApplication.NamespaceID.Value = receivingApplication;
            mshSegment.DateTimeOfMessage.TimeOfAnEvent.Value = GetCurrentTimeStamp();
            mshSegment.MessageType.MessageType.Value = messageTypeMetadata.MessageType;
            mshSegment.MessageType.TriggerEvent.Value = messageTypeMetadata.TriggerEvent;
            mshSegment.MessageType.MessageStructure.Value = messageTypeMetadata.MessageStructure;
            mshSegment.MessageControlID.Value = GuidExtension.NewGuidStringWithoutDashes();
            mshSegment.ProcessingID.ProcessingID.Value = "P";
            mshSegment.VersionID.VersionID.Value = HL7Constants.VERSION_2_3_1;
        }

        public static void ConfigureSegment(PID pidSegment, Patient patient)
        {
            if (patient == null) return;

            //PID-1
            pidSegment.SetIDPID.Value = "1";

            //PID-3
            int? mrnCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PatientIdentifierType, ResourceTypes.MedicalRecordNumber);
            SetIdentifiers(pidSegment, patient.PatientIdentifiers.Where(x => !x.IsDeleted() && x.IdentifierTypeCD == mrnCodeId).ToList());

            //PID-5
            var pid_5 = pidSegment.GetPatientName(0);
            pid_5.FamilyLastName.FamilyName.Value = patient.NameFamily;
            pid_5.GivenName.Value = patient.NameGiven;
            pid_5.MiddleInitialOrName.Value = patient.MiddleName;

            //PID-7
            pidSegment.DateTimeOfBirth.TimeOfAnEvent.Value = GetTimeStamp(patient.BirthDate);

            //PID-8
            pidSegment.Sex.Value = GetAlias(patient.GenderCD);

            //PID-11
            SetAddresses(pidSegment, patient.PatientAddresses.Where(x => !x.IsDeleted()).ToList());

            //PID-13
            SetTelecoms(pidSegment, patient.PatientTelecoms.Where(x => !x.IsDeleted()).ToList(), isPhoneNumber: true);

            //PID-16
            pidSegment.MaritalStatus.Identifier.Value = GetAlias(patient.MaritalStatusCD);

            //PID-17
            pidSegment.Religion.Identifier.Value = GetAlias(patient.ReligionCD);

            //PID-24
            bool? multipleBirth = null;
            if (patient.MultipleBirthId.HasValue)
            {
                multipleBirth = patient.MultipleBirth.isMultipleBorn;
            }
            pidSegment.MultipleBirthIndicator.Value = GetYesNoIndicator(multipleBirth);

            //PID-26
            var citizenship = pidSegment.GetCitizenship(0);
            citizenship.Identifier.Value = GetAlias(patient.CitizenshipCD);

            //PID-29
            pidSegment.PatientDeathDateAndTime.TimeOfAnEvent.Value = GetTimeStamp(patient.DeceasedDateTime);

            //PID-30
            pidSegment.PatientDeathIndicator.Value = GetYesNoIndicator(patient.Deceased);
        }

        public static void ConfigureSegment(PV1 pv1Segment, Encounter encounter)
        {
            //PV1-2
            pv1Segment.PatientClass.Value = GetAlias(encounter.ClassCD);

            //PV1-4
            pv1Segment.AdmissionType.Value = GetAlias(encounter.TypeCD);

            //PV1-7
            SetDoctors(pv1Segment, GetDoctors(encounter, HL7Constants.ATTENDING_DOCTOR), HL7Constants.ATTENDING_DOCTOR);

            //PV1-8
            SetDoctors(pv1Segment, GetDoctors(encounter, HL7Constants.REFERRING_DOCTOR), HL7Constants.REFERRING_DOCTOR);

            //PV1-9
            SetDoctors(pv1Segment, GetDoctors(encounter, HL7Constants.CONSULTING_DOCTOR), HL7Constants.CONSULTING_DOCTOR);

            //PV1-17
            SetDoctors(pv1Segment, GetDoctors(encounter, HL7Constants.ADMITTING_DOCTOR), HL7Constants.ADMITTING_DOCTOR);

            //PV1-19
            SetIdentifier(pv1Segment.VisitNumber, encounter.EncounterIdentifiers?.Where(x => !x.IsDeleted())?.FirstOrDefault());

            //PV1-44
            pv1Segment.AdmitDateTime.TimeOfAnEvent.Value = GetTimeStamp(encounter.AdmissionDate?.DateTime);

            //PV1-45
            pv1Segment.DischargeDateTime.TimeOfAnEvent.Value = GetTimeStamp(encounter.DischargeDate?.DateTime);
        }

        public static void ConfigureSegment(NK1 nk1Segment, PatientContact patientContact)
        {
            //NK1-2
            var name = nk1Segment.GetName(0);
            name.FamilyLastName.FamilyName.Value = patientContact.NameFamily;
            name.GivenName.Value = patientContact.NameGiven;

            //NK1-3
            nk1Segment.Relationship.Identifier.Value = GetAlias(patientContact.ContactRelationshipCD);

            //NK1-4
            SetAddresses(nk1Segment, patientContact.PatientContactAddresses.Where(x => !x.IsDeleted()).ToList());

            //NK1-5
            SetTelecoms(nk1Segment, patientContact.PatientContactTelecoms.Where(x => !x.IsDeleted()).ToList(), isPhoneNumber: true);

            //NK1-7
            nk1Segment.ContactRole.Identifier.Value = GetAlias(patientContact.ContactRoleCD);

            //NK1-8
            nk1Segment.StartDate.Value = GetDate(patientContact.ContactRoleStartDate);

            //NK1-9
            nk1Segment.EndDate.Value = GetDate(patientContact.ContactRoleEndDate);

            //NK1-15
            nk1Segment.Sex.Value = GetAlias(patientContact.GenderCD);

            //NK1-16
            nk1Segment.DateTimeOfBirth.TimeOfAnEvent.Value = GetTimeStamp(patientContact.BirthDate);
        }

        public static void ConfigureSegment(ORC orcSegment)
        {
            orcSegment.OrderControl.Value = "RE";
        }

        public static void ConfigureSegment(OBR obrSegment, FormInstance formInstance, string formAlias)
        {
            //OBR-1
            obrSegment.SetIDOBR.Value = "1";

            //OBR-4
            SetDocumentMetadata(obrSegment.UniversalServiceID, formInstance.Title, formAlias);
            
            //OBR-7
            obrSegment.ObservationDateTime.TimeOfAnEvent.Value = GetTimeStamp(formInstance.EntryDatetime);

            //OBR-25
            obrSegment.ResultStatus.Value = GetAlias(MapFormState(formInstance), (int)CodeSetList.ResultStatus);
        }

        public static void ConfigureSegment(OBX obxSegment, ED encapsulatedData, FormInstance formInstance, PdfDocumentDataOut pdfDocument, string formAlias)
        {
            //OBX-1
            obxSegment.SetIDOBX.Value = "1";

            //OBX-2
            obxSegment.ValueType.Value = "ED"; //value from predefined table

            //OBX-3
            SetDocumentMetadata(obxSegment.ObservationIdentifier, formInstance.Title, formAlias);

            //OBX-5
            SetBinaryData(encapsulatedData, pdfDocument);
            var varies = obxSegment.GetObservationValue(0);
            varies.Data = encapsulatedData;

            //OBX-11
            obxSegment.ObservationResultStatus.Value = GetAlias(MapFormState(formInstance), (int)CodeSetList.ResultStatus);

            //OBX-14
            obxSegment.DateTimeOfTheObservation.TimeOfAnEvent.Value = GetTimeStamp(formInstance.LastUpdate);
        }

        private static void SetBinaryData(ED encapsulatedData, PdfDocumentDataOut pdfDocument)
        {
            encapsulatedData.SourceApplication.NamespaceID.Value = ResourceTypes.SmartOncology;
            encapsulatedData.TypeOfData.Value = "AP"; //value from predefined table
            encapsulatedData.DataSubtype.Value = "PDF";
            encapsulatedData.Encoding.Value = "Base64";

            string base64EncodedStringOfPdfReport = Convert.ToBase64String(pdfDocument.Content);
            encapsulatedData.Data.Value = base64EncodedStringOfPdfReport;
        }

        private static string GetCurrentTimeStamp()
        {
            return GetTimeStamp(DateTime.Now);
        }

        private static string GetTimeStamp(DateTime? dateTime, string format = HL7Constants.HL7_DATE_TIME_FORMAT)
        {
            return dateTime.HasValue ? dateTime.Value.ToString(format, CultureInfo.InvariantCulture) : string.Empty;
        }

        private static string GetDate(DateTime? dateTime)
        {
            return GetTimeStamp(dateTime, "yyyyMMdd");
        }

        private static string GetYesNoIndicator(bool? condition)
        {
            return condition.HasValue ? (condition.Value ? "Y" : "N") : string.Empty;
        }

        private static void SetAddresses(PID pidSegment, List<PatientAddress> patientAddresses)
        {
            for (int i = 0; i < patientAddresses.Count; i++)
            {
                SetAddress(pidSegment.GetPatientAddress(i), patientAddresses[i]);
            }
        }

        private static void SetAddresses(NK1 nk1Segment, List<PatientContactAddress> patientContactAddresses)
        {
            for (int i = 0; i < patientContactAddresses.Count; i++)
            {
                SetAddress(nk1Segment.GetAddress(i), patientContactAddresses[i]);
            }
        }

        private static void SetAddress(XAD hl7Address, AddressBase patientAddress)
        {
            hl7Address.StreetAddress.Value = patientAddress.Street;
            hl7Address.City.Value = patientAddress.City;
            hl7Address.ZipOrPostalCode.Value = patientAddress.PostalCode;
            hl7Address.StateOrProvince.Value = patientAddress.State;
            hl7Address.Country.Value = GetAlias(patientAddress.CountryCD);
            hl7Address.AddressType.Value = GetAlias(patientAddress.AddressTypeCD);
        }

        private static void SetTelecoms(PID pidSegment, List<PatientTelecom> patientTelecoms, bool isPhoneNumber)
        {
            for (int i = 0; i < patientTelecoms.Count; i++)
            {
                if (isPhoneNumber)
                {
                    SetTelecom(pidSegment.GetPhoneNumberHome(i), patientTelecoms[i]);
                }
                else
                {
                    SetTelecom(pidSegment.GetPhoneNumberBusiness(i), patientTelecoms[i]);
                }
            }
        }

        private static void SetTelecoms(NK1 nk1Segment, List<PatientContactTelecom> patientContactTelecoms, bool isPhoneNumber)
        {
            for (int i = 0; i < patientContactTelecoms.Count; i++)
            {
                if (isPhoneNumber)
                {
                    SetTelecom(nk1Segment.GetPhoneNumber(i), patientContactTelecoms[i]);
                }
                else
                {
                    SetTelecom(nk1Segment.GetBusinessPhoneNumber(i), patientContactTelecoms[i]);
                }
            }
        }

        private static void SetTelecom(XTN hl7Telecom, TelecomBase telecom)
        {
            hl7Telecom.Get9999999X99999CAnyText.Value = telecom.Value;
            hl7Telecom.TelecommunicationUseCode.Value = GetAlias(telecom.UseCD);
        }

        private static void SetIdentifiers(PID pidSegment, List<PatientIdentifier> patientIdentifiers)
        {
            for (int i = 0; i < patientIdentifiers.Count; i++)
            {
                SetIdentifier(pidSegment.GetPatientIdentifierList(i), patientIdentifiers[i]);
            }
        }

        private static void SetIdentifier(CX hl7Identifier, IdentifierBase identifier)
        {
            if (identifier == null) return;

            hl7Identifier.IdentifierTypeCode.Value = GetAlias(identifier.IdentifierTypeCD);
            hl7Identifier.ID.Value = identifier.IdentifierValue;
            hl7Identifier.AssigningAuthority.NamespaceID.Value = GetAlias(identifier.IdentifierPoolCD);
        }

        private static void SetDoctors(PV1 pv1Segment, List<PersonnelEncounterRelation> doctorsByRelationType, string relationNameType)
        {
            for (int i = 0; i < doctorsByRelationType.Count; i++)
            {
                switch (relationNameType)
                {
                    case HL7Constants.ADMITTING_DOCTOR:
                        SetDoctor(pv1Segment.GetAdmittingDoctor(i), doctorsByRelationType[i].Personnel);
                        break;
                    case HL7Constants.REFERRING_DOCTOR:
                        SetDoctor(pv1Segment.GetReferringDoctor(i), doctorsByRelationType[i].Personnel);
                        break;
                    case HL7Constants.ATTENDING_DOCTOR:
                        SetDoctor(pv1Segment.GetAttendingDoctor(i), doctorsByRelationType[i].Personnel);
                        break;
                    case HL7Constants.CONSULTING_DOCTOR:
                        SetDoctor(pv1Segment.GetConsultingDoctor(i), doctorsByRelationType[i].Personnel);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void SetDoctor(XCN hl7Doctor, Personnel personnel)
        {
            if (personnel == null) return;

            hl7Doctor.GivenName.Value = personnel.FirstName;
            hl7Doctor.FamilyLastName.FamilyName.Value = personnel.LastName;
            hl7Doctor.MiddleInitialOrName.Value = personnel.MiddleName;
            hl7Doctor.PrefixEgDR.Value = GetAlias(personnel.PrefixCD);

            PersonnelIdentifier personnelIdentifier = personnel.PersonnelIdentifiers.Find(x => !x.IsDeleted());

            SetDoctorIdentifier(hl7Doctor, personnelIdentifier);
        }

        private static void SetDoctorIdentifier(XCN hl7Doctor, PersonnelIdentifier personnelIdentifier)
        {
            if (personnelIdentifier != null)
            {
                hl7Doctor.IdentifierTypeCode.Value = GetAlias(personnelIdentifier.IdentifierTypeCD);
                hl7Doctor.AssigningAuthority.NamespaceID.Value = GetAlias(personnelIdentifier.IdentifierPoolCD);
                hl7Doctor.IDNumber.Value = personnelIdentifier.IdentifierValue;
            }
        }

        private static List<PersonnelEncounterRelation> GetDoctors(Encounter encounter, string relationNameType)
        {
            int relationTypeCodeId = GetDoctorEncounterRelationCodeId(relationNameType);
            return encounter
                .PersonnelEncounterRelations
                .Where(p => p.RelationTypeCD == relationTypeCodeId)
                .ToList();
        }

        private static int GetDoctorEncounterRelationCodeId(string relationNameType)
        {
            return SingletonDataContainer.Instance.GetCodeIdForPreferredTerm(relationNameType, (int)CodeSetList.RelationType);
        }

        private static void SetDocumentMetadata(CE ce, string formTitle, string formAlias)
        {
            ce.Identifier.Value = formAlias;
            ce.Text.Value = formTitle;
        }

        private static string GetAlias(int? codeId)
        {
            return SingletonDataContainer.Instance.GetOutboundAlias(codeId, HL7Constants.HL7_MESSAGES) ?? GetEmptyAliasResponse(codeId);
        }

        private static string GetAlias(string preferredTerm, int codeSetId)
        {
            int? codeId = SingletonDataContainer.Instance.GetCodeId(codeSetId, preferredTerm);
            return GetAlias(codeId);
        }

        private static string GetEmptyAliasResponse(int? codeId)
        {
            return codeId.HasValue ? "Code: " + codeId.ToString() : string.Empty;
        }

        private static string MapFormState(FormInstance formInstance)
        {
            return formInstance?.FormState?.ToString()?.ToLower()?.CapitalizeFirstLetter();
        }
    }
}