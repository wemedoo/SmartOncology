﻿using System.Collections.Generic;
using sReportsV2.Common.Entities.User;
using System.Collections.ObjectModel;

namespace sReportsV2.Initializer
{
    public class IdentifierInitializer
    {
        public static readonly ReadOnlyCollection<string> organizationIdentifiers = new List<string>() { "Accession ID", "Organization identifier", "Provider number", "Social Beneficiary Identifier", "Tax ID number" }.AsReadOnly();
        public static readonly ReadOnlyCollection<string> patientIdentifiers = new List<string>() { "Account number", "Account number Creditor", "Account number debitor", "Accreditation/Certification Identifier",
            "Advanced Practice Registered Nurse number", "American Express", "American Medical Association Number", "Ancestor Specimen ID", "Anonymous identifier",
            "Bank Account Number", "Bank Card Number", "Birth Certificate", "Birth Certificate File Number", "Birth registry number", "Breed Registry Number", "Change of Name Document",
            "Citizenship Card", "Cost Center number", "Country number", "Death Certificate File Number", "Death Certificate ID", "Dentist license number", "Diner's Club card",
            "Diplomatic Passport", "Discover Card", "Doctor number", "Donor Registration Number", "Driver's Licence", "Drug Enforcement Administration registration number",
            "Drug Furnishing or prescriptive authority Number", "Employee number", "Facility ID", "Fetal Death Report File Number", "Fetal Death Report ID", "Filler Identifier",
            "General ledger number", "Guarantor external identifier", "Guarantor internal identifier", "Health Card Number", "Health Plan Identifier", "Indigenous/Aboriginal", "Insel PID",
            "Jurisdictional health number (Canada)", "Labor and industries number", "Laboratory Accession ID", "License number", "Lifelong physician number", "Living Subject Enterprise Number",
            "Local Registry ID", "Marriage Certificate", "Master Card", "Medical License number", "Medical Record Number", "Medicare/CMS (formerly HCFA)'s Universal Physician Identification numbers",
            "Medicare/CMS Performing Provider Identification Number", "Member Number", "Microchip Number", "Military ID number", "National Health Plan Identifier", "National Insurance Organization Identifier",
            "National Insurance Payor Identifier (Payor)", "National Person Identifier where the xxx is the ISO table 3166 3-character (alphabetic) country code", "National employer identifier",
            "National provider identifier", "National unique individual identifier", "Naturalization Certificate", "Nurse practitioner number", "Observation Instance", "Optometrist license number",
            "Osteopathic License number", "Parole Card", "Passport Number", "Patient Medicaid number", "Patient external identifier", "Patient internal identifier", "Patient's Medicare number",
            "Pension Number", "Permanent Resident Card Number", "Person number", "Pharmacist license number", "Physician Assistant number", "Placer Identifier", "Podiatrist license number",
            "Practitioner Medicaid number", "Practitioner Medicare number", "Primary physician office number", "Public Health Case Identifier", "Public Health Event Identifier",
            "Public Health Official ID", "QA number", "Railroad Retirement Provider", "Railroad Retirement number", "Regional registry ID", "Registered Nurse Number", "Resource identifier",
            "Secondary physician office number", "Shipment Tracking Number", "Social Security Number", "Social number", "Specimen ID", "Staff Enterprise Number", "State assigned NDBS card Identifier",
            "State license", "State registry ID", "Study Permit", "Subscriber Number", "Temporary Account Number", "Temporary Living Subject Number", "Temporary Medical Record Number",
            "Temporary Permanent Resident (Canada)", "Training License Number", "Treaty Number/ (Canada)", "Unique Specimen ID", "Unique master citizen number", "Universal Device Identifier",
            "Unspecified identifier", "VISA", "Visit number", "Visitor Permit", "WIC identifier", "Work Permit", "Workers' Comp Number", "Enitentiary/correctional institution Number"}.AsReadOnly();

        //public ThesaurusEntryService thesaurusService = new ThesaurusEntryService();
        //public EnumService enumService = new EnumService();
        private readonly UserData userData = new UserData();

        public IdentifierInitializer()
        {
            userData = CreateUserData();
        }

        //public void InitializeIdentifiers(List<string> identifierList, IdentifierKind identifierKind) 
        //{
        //    foreach (string item in identifierList)
        //    {
        //        if (thesaurusDAL.GetThesaurusCount(item) == 0)
        //            thesaurusDAL.InsertOrUpdate(CreateThesaurusEntry(item), userData);

        //        string thesaurusId = thesaurusDAL.GetByName(item).O40MTId;

        //        if (!codeDAL.GetAll().Where(x => x.Type == identifierKind.ToString()).Any(x => x.ThesaurusId == thesaurusId))
        //            InsertIdentifierType(identifierKind, thesaurusId);
        //    }
        //}

        public UserData CreateUserData() 
        {
            UserData newUserData = new UserData
            {
                ActiveOrganization = 1,
                FirstName = "Mladen",
                LastName = "Stanojevic",
                Username = "smladen"
            };

            return newUserData;
        }
    }
}
