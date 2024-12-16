using NHapi.Base.Model.Primitive;
using NHapi.Model.V231.Datatype;
using NHapi.Model.V231.Segment;
using sReportsV2.Common.Constants;
using sReportsV2.Cache.Singleton;
using sReportsV2.Domain.Sql.Entities.Common;
using SOPatient = sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.Base;
using sReportsV2.Common.Enums;
using sReportsV2.HL7.Constants;

namespace sReportsV2.HL7
{
    public static class HL7IncomingHelper
    {
        public static SOPatient.Patient ProcessPID(PID pidSegment)
        {
            SOPatient.Patient patient = new SOPatient.Patient();
            if (pidSegment == null) return patient;

            var pid_2 = pidSegment.PatientID;
            if (pid_2 != null)
            {
                patient.PatientIdentifiers.Add(GetIdentifier<SOPatient.PatientIdentifier>(pid_2, (int)CodeSetList.PatientIdentifierType));
            }

            var pid_3 = pidSegment.GetPatientIdentifierList();
            if (pid_3 != null)
            {
                patient.PatientIdentifiers.AddRange(GetPatientIdentifiers(pid_3));
            }

            var pid_5 = pidSegment.GetPatientName(0);
            patient.NameFamily = GetValue(pid_5.FamilyLastName.FamilyName.Value);
            patient.NameGiven = GetValue(pid_5.GivenName.Value);
            patient.MiddleName = GetValue(pid_5.MiddleInitialOrName.Value);

            var pid_7 = pidSegment.DateTimeOfBirth?.TimeOfAnEvent;
            if (HasTs(pid_7))
            {
                patient.BirthDate = TsToDateTime(pid_7);
            }

            var pid_8 = pidSegment.Sex;
            if (pid_8 != null)
            {
                patient.GenderCD = GetCodeCDByAlias((int)CodeSetList.Gender, pid_8.Value);
            }

            var pid_11 = pidSegment.GetPatientAddress();
            if (pid_11 != null)
            {
                patient.PatientAddresses.AddRange(GetPatientAddresses<SOPatient.PatientAddress>(pid_11));
            }

            var pid_13 = pidSegment.GetPhoneNumberHome();
            if (pid_13 != null)
            {
                patient.PatientTelecoms.AddRange(GetPatientTelecoms<SOPatient.PatientTelecom>(pid_13));
            }

            var pid_14 = pidSegment.GetPhoneNumberBusiness();
            if (pid_14 != null)
            {
                patient.PatientTelecoms.AddRange(GetPatientTelecoms<SOPatient.PatientTelecom>(pid_14));
            }

            var pid_16 = pidSegment.MaritalStatus;
            if (pid_16 != null)
            {
                patient.MaritalStatusCD = GetCodeCDByAlias((int)CodeSetList.MaritalStatus, pid_16.Identifier.Value);
            }

            var pid_17 = pidSegment.Religion;
            if (pid_17 != null)
            {
                patient.ReligionCD = GetCodeCDByAlias((int)CodeSetList.ReligiousAffiliationType, pid_17.Identifier.Value);
            }

            var pid_24 = pidSegment.MultipleBirthIndicator;
            if (pid_24 != null)
            {
                patient.MultipleBirth = new SOPatient.MultipleBirth(1, IndicatorToBool(pid_24.Value));
            }

            var pid_26 = pidSegment.GetCitizenship(0);
            patient.CitizenshipCD = GetCodeCDByAlias((int)CodeSetList.Citizenship, pid_26.Identifier.Value);

            var pid_29 = pidSegment.PatientDeathDateAndTime?.TimeOfAnEvent;
            if (HasTs(pid_29))
            {
                patient.DeceasedDateTime = TsToDateTime(pid_29);
            }

            var pid_30 = pidSegment.PatientDeathIndicator;
            if (pid_30 != null)
            {
                patient.Deceased = IndicatorToBool(pid_30.Value);
            }

            return patient;
        }

        public static IEnumerable<SOPatient.PatientContact> ProcessNK1s(IEnumerable<NK1> contactData)
        {
            IEnumerable<SOPatient.PatientContact> contacts = new List<SOPatient.PatientContact>();
            if (contactData == null) return contacts;

            contacts = contactData.Select(d => ProcessNK1(d));

            return contacts;
        }

        public static Encounter ProcessPV1(PV1 pv1Segment)
        {
            Encounter encounter = new Encounter()
            {
                PersonnelEncounterRelations = new List<PersonnelEncounterRelation>()
            };
            if (pv1Segment == null) return encounter;

            var pv1_2 = pv1Segment.PatientClass;
            if (pv1_2 != null)
            {
                encounter.ClassCD = GetCodeCDByAlias((int)CodeSetList.EncounterClassification, pv1_2.Value);
            }

            var pv1_4 = pv1Segment.AdmissionType;
            if (pv1_4 != null)
            {
                encounter.TypeCD = GetCodeCDByAlias((int)CodeSetList.EncounterType, pv1_4.Value);
            }

            var pv1_7 = pv1Segment.GetAttendingDoctor();
            if (pv1_7 != null)
            {
                encounter.PersonnelEncounterRelations.AddRange(ProcessDoctors(pv1_7, HL7Constants.ATTENDING_DOCTOR));
            }

            var pv1_8 = pv1Segment.GetReferringDoctor();
            if (pv1_8 != null)
            {
                encounter.PersonnelEncounterRelations.AddRange(ProcessDoctors(pv1_8, HL7Constants.REFERRING_DOCTOR));
            }

            var pv1_9 = pv1Segment.GetConsultingDoctor();
            if (pv1_9 != null)
            {
                encounter.PersonnelEncounterRelations.AddRange(ProcessDoctors(pv1_9, HL7Constants.CONSULTING_DOCTOR));
            }

            var pv1_17 = pv1Segment.GetAdmittingDoctor();
            if (pv1_17 != null)
            {
                encounter.PersonnelEncounterRelations.AddRange(ProcessDoctors(pv1_17, HL7Constants.ADMITTING_DOCTOR));
            }

            var pv1_19 = pv1Segment.VisitNumber;
            if (pv1_19 != null)
            {
                encounter.EncounterIdentifiers.Add(GetIdentifier<EncounterIdentifier>(pv1_19, (int)CodeSetList.PatientIdentifierType));
            }

            var pv1_44 = pv1Segment.AdmitDateTime?.TimeOfAnEvent;
            if (HasTs(pv1_44))
            {
                encounter.AdmissionDate = TsToDateTime(pv1_44);
            }

            var pv1_45 = pv1Segment.DischargeDateTime?.TimeOfAnEvent;
            if (HasTs(pv1_45))
            {
                encounter.DischargeDate = TsToDateTime(pv1_45);
            }

            return encounter;
        }

        private static List<PersonnelEncounterRelation> ProcessDoctors(XCN[] doctorData, string relationTypeName)
        {
            List<PersonnelEncounterRelation> processedDoctors = doctorData
                .Select(d => new PersonnelEncounterRelation()
                {
                    Personnel = ProcessDoctor(d),
                    RelationTypeCD = GetCodeCD((int)CodeSetList.RelationType, relationTypeName)
                })
                .ToList();

            return processedDoctors;
        }

        private static Personnel ProcessDoctor(XCN doctorData)
        {
            string salt = PasswordHelper.CreateSalt(10);
            string timestamp = ((long)DateTime.UtcNow.ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalMilliseconds).ToString();
            string custom_username = $"doctor_{timestamp}";
            string custom_email = $"email_{timestamp}@DOMAIN.com";

            Personnel doctorDb = new Personnel(
                custom_username, 
                PasswordHelper.Hash(ResourceTypes.DefaultPass, salt), 
                salt, 
                custom_email, 
                GetValue(doctorData.GivenName.Value), 
                GetValue(doctorData.FamilyLastName.FamilyName.Value), 
                DateTime.Now
                )
            {
                MiddleName = GetValue(doctorData.MiddleInitialOrName.Value),
                PrefixCD = GetCodeCDByAlias((int)CodeSetList.UserPrefix, doctorData.PrefixEgDR.Value),
                PersonnelTypeCD = GetCodeCD((int)CodeSetList.PersonnelType, ResourceTypes.ExternalPersonnel),
                PersonnelIdentifiers = new List<PersonnelIdentifier> {
                    GetPersonnelIdentifier(doctorData)
                }
            };

            return doctorDb;
        }

        private static SOPatient.PatientContact ProcessNK1(NK1 nk1Segment)
        {
            SOPatient.PatientContact patientContact = new SOPatient.PatientContact();

            var nk1_2 = nk1Segment.GetName(0);
            patientContact.NameFamily = GetValue(nk1_2.FamilyLastName.FamilyName.Value);
            patientContact.NameGiven = GetValue(nk1_2.GivenName.Value);

            var nk1_3 = nk1Segment.Relationship;
            if (nk1_3 != null)
            {
                patientContact.ContactRelationshipCD = GetCodeCDByAlias((int)CodeSetList.ContactRelationship, nk1_3.Identifier.Value);
            }

            var nk1_4 = nk1Segment.GetAddress();
            if (nk1_4 != null)
            {
                patientContact.PatientContactAddresses.AddRange(GetPatientAddresses<SOPatient.PatientContactAddress>(nk1_4));
            }

            var nk1_5 = nk1Segment.GetPhoneNumber();
            if (nk1_5 != null)
            {
                patientContact.PatientContactTelecoms.AddRange(GetPatientTelecoms<SOPatient.PatientContactTelecom>(nk1_5));
            }

            var nk1_6 = nk1Segment.GetBusinessPhoneNumber();
            if (nk1_6 != null)
            {
                patientContact.PatientContactTelecoms.AddRange(GetPatientTelecoms<SOPatient.PatientContactTelecom>(nk1_6));
            }

            var nk1_7 = nk1Segment.ContactRole;
            if (nk1_7 != null)
            {
                patientContact.ContactRoleCD = GetCodeCDByAlias((int)CodeSetList.ContactRole, nk1_7.Identifier.Value);
            }

            var nk1_8 = nk1Segment.StartDate;
            if (HasDt(nk1_8))
            {
                patientContact.ContactRoleStartDate = DtToDateTime(nk1_8);
            }

            var nk1_9 = nk1Segment.EndDate;
            if (HasDt(nk1_9))
            {
                patientContact.ContactRoleEndDate = DtToDateTime(nk1_9);
            }

            var nk1_15 = nk1Segment.Sex;
            if (nk1_15 != null)
            {
                patientContact.GenderCD = GetCodeCDByAlias((int)CodeSetList.Gender, nk1_15.Value);
            }

            var nk1_16 = nk1Segment.DateTimeOfBirth?.TimeOfAnEvent;
            if (HasTs(nk1_16))
            {
                patientContact.BirthDate = TsToDateTime(nk1_16);
            }

            return patientContact;
        }

        private static bool HasTs(TSComponentOne ts)
        {
            return ts != null && !string.IsNullOrEmpty(ts.Value) && ts.Value != HL7Constants.EMPTY_PARSED_STRING;
        }

        public static DateTime TsToDateTime(TSComponentOne ts)
        {
            return new DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, ts.Second, DateTimeKind.Local);
        }

        private static bool HasDt(NHapi.Model.V231.Datatype.DT dt)
        {
            return dt != null && !string.IsNullOrEmpty(dt.Value) && dt.Value != HL7Constants.EMPTY_PARSED_STRING;
        }

        private static DateTime DtToDateTime(NHapi.Model.V231.Datatype.DT dt)
        {
            DateTime result = new DateTime(dt.Year, dt.Month == 0 ? 1 : dt.Month, dt.Day == 0 ? 1 : dt.Day, 0, 0, 0, DateTimeKind.Local);
            return DateTime.SpecifyKind(result, DateTimeKind.Local);
        }

        private static bool IndicatorToBool(string indicator)
        {
            return indicator == "Y";
        }

        private static IEnumerable<SOPatient.PatientIdentifier> GetPatientIdentifiers(CX[] identifiers)
        {
            return identifiers.Select(identifier => GetIdentifier<SOPatient.PatientIdentifier>(identifier, (int)CodeSetList.PatientIdentifierType));
        }

        private static T GetIdentifier<T>(CX identifier, int identifierTypeCodeSetId, int identifierPoolCodeSetId = (int)CodeSetList.IdentifierPool) where T : IdentifierBase, new()
        {
            return new T()
            {
                IdentifierTypeCD = GetCodeCDByAlias(identifierTypeCodeSetId, identifier.IdentifierTypeCode.Value),
                IdentifierValue = GetValue(identifier.ID.Value),
                IdentifierPoolCD = GetCodeCDByAlias(identifierPoolCodeSetId, identifier.AssigningAuthority.NamespaceID.Value)
            };
        }

        private static PersonnelIdentifier GetPersonnelIdentifier(XCN identifier)
        {
            return new PersonnelIdentifier(
                GetCodeCDByAlias((int)CodeSetList.PatientIdentifierType, identifier.IdentifierTypeCode.Value),
                GetValue(identifier.IDNumber.Value)
                )
            {
                IdentifierPoolCD = GetCodeCDByAlias((int)CodeSetList.IdentifierPool, identifier.AssigningAuthority.NamespaceID.Value)
            };
        }

        private static IEnumerable<T> GetPatientTelecoms<T>(XTN[] telecoms) where T : TelecomBase, new()
        {
            return telecoms.Select(telecom => new T()
            {
                Value = GetValue(telecom.Get9999999X99999CAnyText.Value),
                UseCD = GetCodeCDByAlias((int)CodeSetList.TelecommunicationUseType, telecom.TelecommunicationUseCode.Value)
            });
        }

        private static IEnumerable<T> GetPatientAddresses<T>(XAD[] addresses) where T : AddressBase, new()
        {
            return addresses.Select(address =>
             new T()
                {
                    Street = GetValue(address.StreetAddress.Value),
                    City = GetValue(address.City.Value),
                    PostalCode = GetValue(address.ZipOrPostalCode.Value),
                    State = GetValue(address.StateOrProvince.Value),
                    CountryCD = GetCodeCDByAlias((int)CodeSetList.Country, address.Country.Value),
                    AddressTypeCD = GetCodeCDByAlias((int)CodeSetList.AddressType, address.AddressType.Value)
                }
            );
        }

        private static int? GetCodeCDByAlias(int codeSetId, string fieldValue)
        {
            return SingletonDataContainer.Instance.GetCodeIdByAlias(codeSetId, fieldValue);
        }

        private static int? GetCodeCD(int codeSetId, string code)
        {
            return SingletonDataContainer.Instance.GetCodeId(codeSetId, code);
        }

        private static string GetValue(string value)
        {
            return string.IsNullOrEmpty(value) || value == HL7Constants.EMPTY_PARSED_STRING ? null : value;
        }
    }
}