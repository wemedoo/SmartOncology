using NodaTime;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public partial class FieldCalculative
    {
        #region Calculation methods
        public string GetCalculation(Dictionary<string, string> fieldValuesForFormula)
        {
            try
            {
                return this.FormulaType == CalculationFormulaType.Date
                    ? GetDateCalculation(fieldValuesForFormula)
                    : GetNumericCalculation(fieldValuesForFormula);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetNumericCalculation(Dictionary<string, string> fieldValuesForFormula)
        {
            return ParseFormula(GetFormula(fieldValuesForFormula));
        }

        private string GetFormula(Dictionary<string, string> fieldValuesForFormula)
        {
            string formula = this.Formula;
            foreach (KeyValuePair<string, string> fieldValueForFormula in fieldValuesForFormula)
            {
                string fieldVariableName = this.IdentifiersAndVariables[fieldValueForFormula.Key];
                formula = formula.Replace($"[{fieldVariableName}]", fieldValueForFormula.Value);
            }

            return formula;
        }

        private string ParseFormula(string formula)
        {
            object calculatedResult = new DataTable().Compute(formula, "");
            return Math.Round(Convert.ToDouble(calculatedResult), 4).ToString();
        }

        private string GetDateCalculation(Dictionary<string, string> fieldValuesForFormula)
        {
            List<DateTime> dates = GetDates(fieldValuesForFormula);
            if (dates.Count != 2)
            {
                throw new ArgumentException("Invalid formula");
            }

            (LocalDate start, LocalDate end) = ExtractDates(dates);
            return ParseFormula(start, end, this.GranularityType);
        }

        private (LocalDate start, LocalDate end) ExtractDates(List<DateTime> dates)
        {
            DateTime dateFrom, dateTo;
            if (dates[0] >= dates[1])
            {
                dateFrom = dates[1];
                dateTo = dates[0];
            }
            else
            {
                dateFrom = dates[0];
                dateTo = dates[1];
            }

            LocalDate start = new LocalDate(dateFrom.Year, dateFrom.Month, dateFrom.Day);
            LocalDate end = new LocalDate(dateTo.Year, dateTo.Month, dateTo.Day);
            return (start, end);
        }

        private string ParseFormula(LocalDate start, LocalDate end, CalculationGranularityType? granularityType)
        {
            string result = string.Empty;
            Period period;
            switch (granularityType)
            {
                case CalculationGranularityType.Day:
                    period = Period.Between(start, end, PeriodUnits.Days);
                    result = FormatDatePartDifference(period.Days, "Day");
                    break;
                case CalculationGranularityType.Month:
                    period = Period.Between(start, end, PeriodUnits.Months);
                    result = FormatDatePartDifference(period.Months, "Month");
                    break;
                case CalculationGranularityType.Year:
                    period = Period.Between(start, end, PeriodUnits.Years);
                    result = FormatDatePartDifference(period.Years, "Year");
                    break;
                case CalculationGranularityType.YearMonth:
                    period = Period.Between(start, end);
                    result = $"{FormatDatePartDifference(period.Years, "Year")}, {FormatDatePartDifference(period.Months, "Month")}";
                    break;
                case CalculationGranularityType.YearMonthDay:
                    period = Period.Between(start, end);
                    result = $"{FormatDatePartDifference(period.Years, "Year")}, {FormatDatePartDifference(period.Months, "Month")}, {FormatDatePartDifference(period.Days, "Day")}";
                    break;
                default:
                    break;
            }

            return result;
        }

        private string FormatDatePartDifference(int datePartValue, string datePart)
        {
            return $"{datePartValue} {datePart}{AddPlural(datePartValue)}";
        }

        private string AddPlural(int number)
        {
            return number > 1 ? "s" : "";
        }

        private List<DateTime> GetDates(Dictionary<string, string> fieldValuesForFormula)
        {
            List<DateTime> dates = new List<DateTime>();

            foreach (KeyValuePair<string, string> fieldValueForFormula in fieldValuesForFormula)
            {
                if (DateTime.TryParse(fieldValueForFormula.Value, out DateTime dateValue))
                {
                    dates.Add(dateValue);
                }
            }

            return dates;
        }

        #endregion /Calculation methods
    }
}
