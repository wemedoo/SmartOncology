using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace sReportsV2.Initializer.CodeSets
{
    public class CodeSetsImporter
    {
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICodeSetDAL codeSetDAL;
        public CodeSetsImporter() { }

        public CodeSetsImporter(ICodeSetDAL codeSetDAL, IThesaurusDAL thesaurusDAL)
        {
            this.thesaurusDAL = thesaurusDAL;
            this.codeSetDAL = codeSetDAL;
        }

        public void Import() 
        {
            List<string> codeSets = new List<string>();
            var codeSetProperties = Array.Find(Assembly.GetAssembly(typeof(CodeSetValues)).GetTypes(), x => x.Name == "CodeSetValues")?.GetProperties();

            if (codeSetProperties != null)
                foreach (var codeSet in codeSetProperties)
                    codeSets.Add(codeSet.CustomAttributes?.FirstOrDefault()?.ConstructorArguments?.FirstOrDefault().Value.ToString());

            InsertCodeSets(codeSets);
        }

        private void InsertCodeSets(List<string> codeSets)
        {
            Dictionary<string, int> terms = new Dictionary<string, int>();
            int nextCodeSetId = codeSetDAL.GetAll().Select(x => x.CodeSetId).OrderByDescending(id => id).FirstOrDefault() + 1;
            foreach (var codeSet in codeSets) 
            {
                if (codeSetDAL.GetAll().Where(x => x.ThesaurusEntry.Translations
                    .Any(m => m.PreferredTerm == codeSet)).Count() == 0)          
                {
                    if(codeSet == CodeSetAttributeNames.EntityState)
                        terms.Add(codeSet, 2000);
                    else
                        terms.Add(codeSet, nextCodeSetId++);
                }
            }
            InsertData(terms);
        }

        private void InsertData(Dictionary<string, int> codeSets) 
        {

            foreach (KeyValuePair<string, int> codeSet in codeSets)
            {
                string term = codeSet.Key;
                int thesaurusId;
                ThesaurusEntry thesaurusEntryDb = thesaurusDAL.GetByPreferredTerm(term);

                if (thesaurusEntryDb != null)
                {
                    thesaurusId = thesaurusEntryDb.ThesaurusEntryId;

                }
                else
                {
                    ThesaurusEntry thesaurus = new ThesaurusEntry()
                    {
                        Translations = new List<ThesaurusEntryTranslation>()
                        {
                            new ThesaurusEntryTranslation()
                            {
                                Language = LanguageConstants.EN,
                                PreferredTerm = term,
                                Definition = term
                            }
                        }
                    };
                    thesaurusDAL.InsertOrUpdate(thesaurus);
                    thesaurusId = thesaurus.ThesaurusEntryId;
                }
                
                codeSetDAL.Insert(new Domain.Sql.Entities.Common.CodeSet()
                {
                    CodeSetId=codeSet.Value,
                    ThesaurusEntryId = thesaurusId
                });
            }
        }
    }
}
