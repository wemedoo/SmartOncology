using sReportsV2.Common.Configurations;
using sReportsV2.Common.Constants;
using System;
using System.Globalization;
using System.Linq;

namespace sReportsV2.Common.Extensions
{
    public static class DateTimeExtension
    {
        public static readonly string DefaultTimezone = "Europe/Zurich";

        #region DateTime extensions
        public static DateTime GetCurrentDateTime(string timeZone)
        {
            return DateTime.Now.ToTimeZoned(timeZone);
        }

        public static DateTime ToTimeZoned(this DateTime dateTime, string timeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, GetTimeZoneInfo(timeZone));
        }

        public static string ToTimeZonedDateTime(this DateTime? dateTime, string timeZone, string dateFormat)
        {
            return dateTime.HasValue ? ToTimeZoned(dateTime.Value, timeZone, dateFormat) : string.Empty;
        }

        public static string ToTimeZoned(this DateTime dateTime, string timeZone, string dateFormat)
        {
            return ToTimeZoned(dateTime, timeZone).GetDateTimeDisplay(dateFormat);
        }

        public static string ToTimeZonedDatePart(this DateTime dateTime, string timeZone, string dateFormat)
        {
            return ToTimeZoned(dateTime, timeZone).GetDateTimeDisplay(dateFormat, excludeTimePart: true);
        }

        public static string GetTimePart(this DateTime date)
        {
            return date.ToString(DateTimeConstants.TimeFormat);
        }

        public static string GetTimePart(this DateTime? date)
        {
            return date.HasValue ? date.Value.GetTimePart() : string.Empty;
        }

        public static string GetDateTimeDisplay(this DateTime? date, string dateFormat, bool excludeTimePart = false, bool showSeconds = false)
        {
            return date.HasValue ? date.Value.GetDateTimeDisplay(dateFormat, excludeTimePart, showSeconds) : string.Empty;
        }

        public static string GetDateTimeDisplay(this DateTime date, string dateFormat, bool excludeTimePart = false, bool showSeconds = false)
        {
            string timePart = excludeTimePart ? string.Empty : $" {date.GetTimePart()}";
            timePart = excludeTimePart || !showSeconds ? timePart : $"{timePart}:{date:ss.fff}";
            return $"{date.ToString(dateFormat, CultureInfo.InvariantCulture)}{timePart}";
        }

        public static DateTime AppendDays(this DateTime date, int dayNumber)
        {
            return date.AddDays(GetDayOffset(dayNumber));
        }

        private static bool IsDateInUtcFormat(string datePart, out DateTime parsedDate)
        {
            return DateTime.TryParseExact(datePart, DateTimeConstants.UTCDatePartFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }
        #endregion /DateTime extensions

        #region DateTimeOffset extensions
        public static string ToTimeZoned(this DateTimeOffset? dateTime, string dateFormat, string timezoneOffset = null, bool seconds = false, bool milliseconds = false)
        {
            return dateTime.HasValue ? dateTime.Value.ToTimeZoned(dateFormat, timezoneOffset, seconds, milliseconds) : string.Empty;
        }

        public static string ToTimeZoned(this DateTimeOffset dateTime, string dateFormat, string customTimezoneId = null, bool seconds = false, bool milliseconds = false)
        {
            string timeZoneId = string.IsNullOrEmpty(customTimezoneId) ? GlobalConfig.GetTimeZoneId() : customTimezoneId;
            DateTimeOffset localTime = dateTime.GetDateTimeByTimeZone(timeZoneId);

            var datePart = localTime.ToDateZoned(dateFormat);
            var timePart = localTime.GetTimePart();

            string dateTimeString = $"{datePart} {timePart}";
            if (seconds)
            {
                dateTimeString += $":{localTime:ss}";
            }
            if (milliseconds)
            {
                dateTimeString += $".{localTime:fff}";
            }
            return dateTimeString;
        }

        public static string ToDateZoned(this DateTimeOffset dateTime, string dateFormat)
        {
            var datePart = dateTime.ToString(dateFormat, CultureInfo.InvariantCulture);
            return datePart;
        }

        public static string ToDateZoned(this DateTimeOffset? dateTime, string dateFormat)
        {
            return dateTime.HasValue ? dateTime.Value.ToDateZoned(dateFormat) : string.Empty;
        }

        public static string GetTimePart(this DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.GetTimePart() : string.Empty;
        }

        public static string GetTimePart(this DateTimeOffset date)
        {
            return date.ToString(DateTimeConstants.TimeFormat);
        }

        public static string ToActiveToDateTimeFormat(this DateTimeOffset dateTime, string dateFormat)
        {
            return dateTime.Date != DateTimeOffset.MaxValue.Date ? dateTime.ToTimeZoned(dateFormat) : string.Empty;
        }

        public static string ToActiveToDateFormat(this DateTimeOffset dateTime, string dateFormat)
        {
            return dateTime.Date != DateTimeOffset.MaxValue.Date ? dateTime.ToDateZoned(dateFormat) : string.Empty;
        }

        public static string ConvertFormInstanceDateTimeToOrganizationTimeZone(this DateTimeOffset date)
        {
            return date.GetDateTimeByTimeZone(GlobalConfig.GetTimeZoneId()).ToString(DateTimeConstants.UTCZonedFormat);
        }

        public static DateTimeOffset GetDateTimeByTimeZone(this DateTimeOffset dateTime, string timeZoneId)
        {
            return TimeZoneInfo.ConvertTime(dateTime, GetTimeZoneInfo(timeZoneId));
        }

        public static string GetDateTimeDisplay(this DateTimeOffset? date, string dateFormat)
        {
            return date.HasValue ? date.Value.GetDateTimeDisplay(dateFormat) : string.Empty;
        }

        public static string GetDateTimeDisplay(this DateTimeOffset date, string dateFormat)
        {
            var datePart = date.ToDateZoned(dateFormat);
            var timePart = date.GetTimePart();

            return $"{datePart} {timePart}";
        }

        public static DateTimeOffset? ConvertToOrganizationTimeZone(this DateTimeOffset? date)
        {
            return date.HasValue ? date.Value.ConvertToOrganizationTimeZone() : date;
        }

        public static DateTimeOffset ConvertToOrganizationTimeZone(this DateTimeOffset date, string organizationTimeZone = null)
        {
            return date.GetDateTimeByTimeZone(GlobalConfig.GetTimeZoneId(organizationTimeZone));
        }

        #endregion /DateTimeOffset extensions

        #region Offset extensions
        public static string GetOffsetValue(string customOrganizationTimeZoneId = null, bool showPlusForPositiveValues = true)
        {
            string timeZoneId = string.IsNullOrEmpty(customOrganizationTimeZoneId) ? GlobalConfig.GetTimeZoneId() : customOrganizationTimeZoneId;
            return GetTimeZoneInfo(timeZoneId).GetUtcOffset(DateTime.UtcNow).GetOffsetValue(showPlusForPositiveValues);
        }

        private static string GetOffsetValue(this TimeSpan offset, bool showPlusForPositiveValues)
        {
            string sign = offset >= TimeSpan.Zero ? (showPlusForPositiveValues ? "+" : string.Empty) : "-";
            return $"{sign}{offset:hh\\:mm}";
        }

        private static int GetDayOffset(int dayNumber)
        {
            int sign = dayNumber > 0 ? -1 : 0;
            return dayNumber + 1 * sign;
        }

        public static string GetTimeZoneInfoId(this string timeZoneDisplayName)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.GetSystemTimeZones().First(tz => tz.DisplayName == timeZoneDisplayName);

            return timeZone.Id;
        }

        public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId ?? DefaultTimezone);
        }
        #endregion /Offset extensions

        #region DateTime(-offset) string extensions

        public static string RenderDate(this string dateTimeValue)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeValue))
            {
                string[] dateTimeParts = dateTimeValue.Split('T');
                string datePart = dateTimeParts[0];
                datePart = HandleValueDuplication(datePart);

                if (IsDateInUtcFormat(datePart, out DateTime parsedDate))
                {
                    return parsedDate.GetDateTimeDisplay(DateTimeConstants.DateFormat, excludeTimePart: true);
                }
                else
                {
                    return datePart;
                }
            }
            else
            {
                return "";
            }
        }

        public static string RenderTime(this string dateTimeValue)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeValue))
            {
                string[] dateTimeParts = dateTimeValue.Split('T');
                string timeWithZonePart = dateTimeParts.Length == 2 ? dateTimeParts[1] : "";
                var timeWithoutDuplication = HandleValueDuplication(timeWithZonePart);
                string[] timePart = timeWithoutDuplication.Split('-', '+');
                return timePart[0];
            }
            else
            {
                return "";
            }
        }

        public static string RenderDatetime(this string dateTimeValue)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeValue) && DateTimeOffset.TryParse(dateTimeValue, out DateTimeOffset parsedDate))
            {
                return parsedDate.GetDateTimeDisplay(DateTimeConstants.DateFormat);
            }
            return "";
        }

        public static bool HasOffset(this string dateTimeString)
        {
            int tIndex = dateTimeString.IndexOf('T');
            if (tIndex == -1) return false;

            string timePart = dateTimeString.Substring(tIndex + 1);

            return timePart.Contains("+") || timePart.Contains("-") || timePart.EndsWith('Z');
        }

        private static string HandleValueDuplication(string dateTimeValue)
        {
            return dateTimeValue.Contains(',') ? dateTimeValue.Split(',')[0] : dateTimeValue;
        }
        #endregion /DateTime(-offset) string extensions
    }
}