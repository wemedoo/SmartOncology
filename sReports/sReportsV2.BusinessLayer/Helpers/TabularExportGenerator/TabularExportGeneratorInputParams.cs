using sReportsV2.Cache.Resources;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.File.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Helpers.TabularExportGenerator
{
    public class TabularExportGeneratorInputParams
    {
        public TabularExportGeneratorInputParams(FileWriter fileWriter, Form currentForm, UserCookieData userCookieData, string dateFormat, KeyValuePair<int, string> organization, Dictionary<int, Dictionary<int, string>> missingValues, int batchSize = 10000, object repetitiveElement = null)
        {
            FileWriter = fileWriter;
            CurrentForm = currentForm;
            BatchSize = batchSize;
            TimeZoneOffset = userCookieData.TimeZoneOffset;
            Organization = organization;
            DateFormat = dateFormat;
            RepetitiveElement = repetitiveElement;
            MissingValues = missingValues;
        }

        public FileWriter FileWriter { get; set; }
        public Form CurrentForm { get; set; }
        public string TimeZoneOffset { get; set; }
        public KeyValuePair<int, string> Organization { get; set; }
        public string DateFormat { get; set; }
        public object RepetitiveElement { get; set; }
        public int BatchSize { get; set; } = 10000;
        public Dictionary<int, Dictionary<int, string>> MissingValues { get; set; }
    }
}
