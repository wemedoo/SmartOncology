namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.Common;
    using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.SqlClient;
    using System.Linq;


    public partial class AddClinicalDomainCodeSet : DbMigration
    {
        private const string DefaultDateTime = "9999-12-31 23:59:59.9999999 +00:00";

        public override void Up()
        {
            using (SReportsContext dbContext = new SReportsContext())
            {
                bool hasEntities = dbContext.Modules.Any() || dbContext.Permissions.Any();
                if (hasEntities)
                {
                    int codeSetId = CreateCodeSet(dbContext, CodeSetAttributeNames.ClinicalDomain, (int)CodeSetList.ClinicalDomain);

                    foreach (var clinicalDomain in ClinicalDomain)
                    {
                        GetOrCreateCodeByPreferredTerm(dbContext, codeSetId, clinicalDomain);
                    }
                }
            }
        }
        
        public override void Down()
        {
        }

        private int CreateCodeSet(SReportsContext dbContext, string codeSetName, int codeSetId)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codeSetName);

            dbContext.Database.ExecuteSqlCommand($@"
            insert into CodeSets (CodeSetId, ThesaurusEntryId, EntryDatetime, EntityStateCD, ActiveFrom, ActiveTo) 
            values ({codeSetId}, {thesaurusId}, GETDATE(), {GetActiveStateCodeId(dbContext)}, SYSDATETIMEOFFSET(), '{DefaultDateTime}');
            ");

            return codeSetId;
        }

        private int GetOrCreateCodeByPreferredTerm(SReportsContext dbContext, int codeSetId, string codePreferredTerm)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codePreferredTerm);
            int codeId = GetNextCodeId(dbContext);

            InsertNewCode(dbContext, codeId, thesaurusId, codeSetId);

            return codeId;
        }

        private int GetNextCodeId(SReportsContext dbContext)
        {
            const string selectNextCodeIdQuery = @"SELECT TOP (1) CodeId FROM Codes ORDER BY CodeId DESC";
            return dbContext.Database.SqlQuery<int>(selectNextCodeIdQuery).FirstOrDefault() + 1;
        }

        private void InsertNewCode(SReportsContext dbContext, int codeId, int thesaurusId, int codeSetId)
        {
            const string insertCodeQuery = @"
                SET IDENTITY_INSERT dbo.Codes ON;
                INSERT INTO Codes (CodeId, ThesaurusEntryId, CodeSetId, EntryDatetime, EntityStateCD, ActiveFrom, ActiveTo)
                VALUES (@CodeId, @ThesaurusEntryId, @CodeSetId, GETDATE(), @EntityStateCD, SYSDATETIMEOFFSET(), @DefaultDateTime);
                SET IDENTITY_INSERT dbo.Codes OFF;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CodeId", codeId),
                new SqlParameter("@ThesaurusEntryId", thesaurusId),
                new SqlParameter("@CodeSetId", codeSetId),
                new SqlParameter("@EntityStateCD", GetActiveStateCodeId(dbContext)),
                new SqlParameter("@DefaultDateTime", DefaultDateTime)
            };

            dbContext.Database.ExecuteSqlCommand(insertCodeQuery, parameters);
        }

        private int GetOrCreateThesaurusId(SReportsContext dbContext, string preferredTerm, string definition = null)
        {
            const string selectQuery = "SELECT ThesaurusEntryId FROM ThesaurusEntryTranslations WHERE PreferredTerm = @PreferredTerm";

            var parameters = new SqlParameter("@PreferredTerm", preferredTerm);
            int thesaurusId = dbContext.Database.SqlQuery<int>(selectQuery, parameters).FirstOrDefault();

            if (thesaurusId <= 0)
            {
                thesaurusId = InsertNewThesaurusEntry(dbContext);

                InsertThesaurusEntryTranslation(dbContext, thesaurusId, preferredTerm, definition);
            }

            return thesaurusId;
        }

        private int InsertNewThesaurusEntry(SReportsContext dbContext)
        {
            const string insertQuery = @"
                INSERT INTO ThesaurusEntries (EntryDatetime, EntityStateCD, ActiveFrom, ActiveTo)
                VALUES (GETDATE(), @EntityStateCD, SYSDATETIMEOFFSET(), @DefaultDateTime);
                SELECT SCOPE_IDENTITY()";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EntityStateCD", GetActiveStateCodeId(dbContext)),
                new SqlParameter("@DefaultDateTime", DefaultDateTime)
            };

            return (int)dbContext.Database.SqlQuery<decimal>(insertQuery, parameters).FirstOrDefault();
        }

        private void InsertThesaurusEntryTranslation(SReportsContext dbContext, int thesaurusId, string preferredTerm, string definition)
        {
            const string insertTranslationQuery = @"
                INSERT INTO ThesaurusEntryTranslations (ThesaurusEntryId, Language, PreferredTerm, Definition)
                VALUES (@ThesaurusEntryId, @Language, @PreferredTerm, @Definition)";

            var translationParameters = new SqlParameter[]
            {
                new SqlParameter("@ThesaurusEntryId", thesaurusId),
                new SqlParameter("@Language", LanguageConstants.EN),
                new SqlParameter("@PreferredTerm", preferredTerm),
                new SqlParameter("@Definition", definition ?? preferredTerm)
            };

            dbContext.Database.ExecuteSqlCommand(insertTranslationQuery, translationParameters);
        }

        private int GetActiveStateCodeId(SReportsContext dbContext)
        {
            string selectQuery = "SELECT TOP (1) CodeId FROM Codes where CodeSetId = @CodeSetId";
            var parameters = new SqlParameter("@CodeSetId", (int)CodeSetList.EntityState);
            int codeId = dbContext.Database.SqlQuery<int>(selectQuery, parameters).FirstOrDefault();

            return codeId;
        }

        public static List<string> ClinicalDomain { get; set; } = new List<string>()
        {
            "Allergy",
            "Acupuncture",
            "Addiction Medicine",
            "Addiction Psychiatry",
            "Adolescent Medicine",
            "Aerodigestive Medicine",
            "Aerospace Medicine",
            "Anesthesiology",
            "Audiology",
            "Bariatric Surgery",
            "Birth Defects",
            "Blood Banking And Transfusion Medicine",
            "Bone Marrow Transplant",
            "Brain Injury",
            "Burn Management",
            "Cardiac Surgery",
            "Cardiovascular Disease",
            "Chemical Pathology",
            "Child And Adolescent Psychiatry",
            "Child And Adolescent Psychology",
            "Chiropractic Medicine",
            "Cleft And Craniofacial",
            "Clinical Biochemical Genetics",
            "Clinical Cardiac Electrophysiology",
            "Clinical Genetics",
            "Clinical Pathology",
            "Clinical Pharmacology",
            "Colon And Rectal Surgery",
            "Community Health Care",
            "Critical Care Medicine",
            "Dentistry",
            "Developmental Behavioral Pediatrics",
            "Diabetology",
            "Dialysis",
            "Eating Disorders",
            "Emergency Medicine",
            "Environmental Health",
            "Epilepsy",
            "Ethics",
            "Family Medicine",
            "Forensic Medicine",
            "General Medicine",
            "Geriatric Medicine",
            "Gynecologic Oncology",
            "Gynecology",
            "Heart Failure",
            "Hepatology",
            "HIV",
            "Infectious Disease",
            "Integrative Medicine",
            "Interventional Cardiology",
            "Interventional Radiology",
            "Kinesiotherapy",
            "Maternal And Fetal Medicine",
            "Medical Aid In Dying",
            "Medical Genetics",
            "Medical Microbiology Pathology",
            "Medical Oncology",
            "Medical Toxicology",
            "Mental Health",
            "Multi Specialty Program",
            "Neonatal Perinatal Medicine",
            "Neurological Surgery",
            "Neurology W Special Qualifications In Child Neuro",
            "Neuropsychology",
            "Nutrition And Dietetics",
            "Obesity Medicine",
            "Obstetrics",
            "Obstetrics And Gynecology",
            "Occupational Therapy",
            "Optometry",
            "Orthopaedic Surgery",
            "Orthotics Prosthetics",
            "Otolaryngology",
            "Pain Medicine",
            "Palliative Care",
            "Pastoral Care",
            "Pediatric Cardiology",
            "Pediatric Critical Care Medicine",
            "Pediatric Dermatology",
            "Pediatric Endocrinology",
            "Pediatric Gastroenterology",
            "Pediatric Hematology Oncology",
            "Pediatric Infectious Diseases",
            "Pediatric Nephrology",
            "Pediatric Otolaryngology",
            "Pediatric Pulmonology",
            "Pediatric Rehabilitation Medicine",
            "Pediatric Rheumatology",
            "Pediatrics",
            "Pediatric Surgery",
            "Pediatric Transplant Hepatology",
            "Pediatric Urology",
            "Pharmacogenomics",
            "Physical Medicine And Rehab",
            "Physical Therapy",
            "Podiatry",
            "Polytrauma",
            "Primary Care",
            "Psychology",
            "Pulmonary Disease",
            "Recreational Therapy",
            "Reproductive Endocrinology And Infertility",
            "Research",
            "Respiratory Therapy",
            "Sleep Medicine",
            "Solid Organ Transplant",
            "Speech Language Pathology",
            "Spinal Cord Injury Medicine",
            "Spinal Surgery",
            "Sports Medicine",
            "Surgery",
            "Surgery Of The Hand",
            "Surgical Critical Care",
            "Surgical Oncology",
            "Therapeutic Apheresis",
            "Thoracic And Cardiac Surgery",
            "Thromboembolism",
            "Transplant Cardiology",
            "Transplant Surgery",
            "Trauma",
            "Tumor Board",
            "Undersea And Hyperbaric Medicine",
            "Vascular Neurology",
            "Vocational Rehabilitation",
            "Women's Health",
            "Wound Care Management",
            "Wound Ostomy And Continence Care",
            "Accident And Emergency Medicine",
            "Allergology",
            "Anaesthetics",
            "Cardiology",
            "Child Psychiatry",
            "Clinical Biology",
            "Clinical Chemistry",
            "Clinical Neurophysiology",
            "Craniofacial Surgery",
            "Dermatology",
            "Endocrinology",
            "Family And General Medicine",
            "Gastroenterologic Surgery",
            "Gastroenterology",
            "General Practice",
            "General Surgery",
            "Geriatrics",
            "Hematology",
            "Immunology",
            "Infectious Diseases",
            "Internal Medicine",
            "Laboratory Medicine",
            "Microbiology",
            "Nephrology",
            "Neuropsychiatry",
            "Neurology",
            "Neurosurgery",
            "Nuclear Medicine",
            "Obstetrics And Gynaecology",
            "Occupational Medicine",
            "Oncology",
            "Ophthalmology",
            "Oral And Maxillofacial Surgery",
            "Orthopaedics",
            "Otorhinolaryngology",
            "Paediatric Surgery",
            "Paediatrics",
            "Pathology",
            "Pharmacology",
            "Physical Medicine And Rehabilitation",
            "Plastic Surgery",
            "Podiatric Surgery",
            "Preventive Medicine",
            "Psychiatry",
            "Public Health",
            "Radiation Oncology",
            "Radiology",
            "Respiratory Medicine",
            "Rheumatology",
            "Stomatology",
            "Thoracic Surgery",
            "Tropical Medicine",
            "Urology",
            "Vascular Surgery",
            "Venereology",
            "Pre Clinical Research",
            "Veterinary Medicine",
            "Clinical Microbiology"
        };
    }
}
