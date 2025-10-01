using sReportsV2.Cache.Resources;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Helpers.TabularExportGenerator
{
    public class FormThesaurusReportExportGenerator
    {
        public TabularExportGeneratorInputParams InputParams { get; set; }
        public List<FormThesaurusReportView> FormThesaurusReportViews { get; set; }

        public FormThesaurusReportExportGenerator(TabularExportGeneratorInputParams inputParams, List<FormThesaurusReportView> formThesaurusReportViews)
        {
            InputParams = inputParams;
            FormThesaurusReportViews = formThesaurusReportViews;
        }

        public void GenerateReportInExcel()
        {
            InputParams.FileWriter.WriteRow(GenerateHeaderRow());

            foreach (FormThesaurusReportView formThesaurusReportView in FormThesaurusReportViews)
            {
                InputParams.FileWriter.WriteRow(new List<string>()
                    {
                        formThesaurusReportView.ThesaurusEntryId.ToString(),
                        formThesaurusReportView.PreferredTerm,
                        formThesaurusReportView.System,
                        formThesaurusReportView.Version,
                        formThesaurusReportView.Code,
                        formThesaurusReportView.Value,
                        formThesaurusReportView.VersionPublishDate.HasValue ? formThesaurusReportView.VersionPublishDate.Value.ToString(InputParams.DateFormat) : string.Empty,
                        InputParams.CurrentForm.Title
                    }
                );
            }
        }

        private List<string> GenerateHeaderRow()
        {
            return new List<string>() {
                    $"{TextLanguage.Thesaurus} ID",
                    TextLanguage.Preferred_term.CapitalizeFirstLetterInEveryWord(),
                    TextLanguage.System,
                    TextLanguage.Version,
                    TextLanguage.Code,
                    TextLanguage.Value,
                    TextLanguage.VersionPublishDate.CapitalizeFirstLetterInEveryWord(),
                    $"{TextLanguage.Document} {TextLanguage.Name}"
                    
                };
        }
    }
}