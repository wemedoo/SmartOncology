using sReportsV2.Common.Configurations;
using sReportsV2.Common.Constants;
using System;
using System.Globalization;

namespace sReportsV2.Common.Extensions
{
    public static class DateTimeExtension
    {
        private static readonly string defaultTimezone = "Europe/London";
        public static DateTime? ToTimeZonedDateTime(this DateTime? dateTime, string timeZone)
        {
            if (dateTime.HasValue) 
            {
                return ToTimeZoned(dateTime.Value, timeZone);
            } 
            else
            {
                return null;
            }
        }

        public static DateTime ToTimeZoned(this DateTime dateTime, string timeZone)
        {
            TimeZoneInfo windowsTZ = TimeZoneInfo.FindSystemTimeZoneById(timeZone ?? defaultTimezone);
            DateTime utc = dateTime.ToUniversalTime();
            return TimeZoneInfo.ConvertTimeFromUtc(utc, windowsTZ);
        }

        public static string ToTimeZoned(this DateTimeOffset? dateTime, string dateFormat, string timezoneOffset = null, bool seconds = false, bool milliseconds = false)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToTimeZoned(dateFormat, timezoneOffset, seconds, milliseconds);
            }
            else 
            {
                return null;
            }
        }
        
        public static string ToTimeZonedTime(this DateTimeOffset? dateTime, string dateFormat)
        {
            if (dateTime.HasValue)
            {
                TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
                var localTime = dateTime.Value.ToUniversalTime().ToOffset(timeSpan);
                var timePart = localTime.ToString("HH:mm");

                return $"{timePart}";
            }
            else 
            {
                return null;
            }
        }

        public static string ToTimeZonedDate(this DateTimeOffset? dateTime, string dateFormat)
        {
            if (dateTime.HasValue)
            {
                TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
                var localTime = dateTime.Value.ToUniversalTime().ToOffset(timeSpan);
                var datePart = localTime.ToString(dateFormat, CultureInfo.InvariantCulture);

                return $"{datePart}";
            }
            else 
            {
                return null;
            }
        }

        public static string ToTimeZoned(this DateTimeOffset dateTime, string dateFormat, string timezoneOffset = null, bool seconds = false, bool milliseconds = false)
        {
            TimeSpan timeSpan = TimeSpan.Parse(GetTimezoneOffset(timezoneOffset));
            var localTime = dateTime.ToUniversalTime().ToOffset(timeSpan);
            var datePart = localTime.ToString(dateFormat, CultureInfo.InvariantCulture);
            var timePart = localTime.ToString("HH:mm");

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
            if (dateTime.HasValue)
            {
                TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
                var localTime = dateTime.Value.ToUniversalTime().ToOffset(timeSpan);
                var datePart = localTime.ToString(dateFormat, CultureInfo.InvariantCulture);

                return datePart;
            }
            else 
            {
                return null;
            }
        }

        public static DateTime AppendDays(this DateTime date, int dayNumber)
        {
            return date.AddDays(GetOffset(dayNumber));
        }

        private static int GetOffset(int dayNumber)
        {
            int sign = dayNumber > 0 ? -1 : 0;
            return dayNumber + 1 * sign;
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
            string fullTimePart = date.ToString("s").Split('T')[1];
            int secondsSeparatorIndex = fullTimePart.LastIndexOf(':');
            return fullTimePart.Substring(0, secondsSeparatorIndex);
        }

        public static string GetTimePart(this DateTimeOffset date)
        {
            TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
            date.ToUniversalTime().ToOffset(timeSpan);
            string fullTimePart = date.ToString("s").Split('T')[1];
            int secondsSeparatorIndex = fullTimePart.LastIndexOf(':');
            return fullTimePart.Substring(0, secondsSeparatorIndex);
        }

        public static string RenderDate(this string dateTimeValue)
        {
            if (!string.IsNullOrWhiteSpace(dateTimeValue))
            {
                string[] dateTimeParts = dateTimeValue.Split('T');
                string datePart = dateTimeParts[0];
                datePart = HandleValueDuplication(datePart);

                if(IsDateInUtcFormat(datePart, out DateTime parsedDate))
                {
                    return parsedDate.GetDateTimeDisplay(DateConstants.DateFormat, excludeTimePart: true);
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
                return parsedDate.GetDateTimeDisplay(DateConstants.DateFormat);
            }
            return "";
        }

        public static string ConvertNullableDateForDisplay(this DateTime? dateTime, string dateFormat)
        {
            return dateTime.HasValue ? dateTime.Value.ToString(dateFormat, CultureInfo.InvariantCulture) : string.Empty;
        }

        private static string HandleValueDuplication(string dateTimeValue)
        {
            return dateTimeValue.Contains(',') ? dateTimeValue.Split(',')[0] : dateTimeValue;
        }

        public static string ToActiveToFormat(this DateTime dateTime, string dateFormat)
        {
            return dateTime.Date != DateTime.MaxValue.Date ? dateTime.ToString(dateFormat, CultureInfo.InvariantCulture) : string.Empty;
        }

        public static string ToActiveToDateTimeFormat(this DateTimeOffset dateTime, string dateFormat)
        {
            return dateTime.Date != DateTimeOffset.MaxValue.Date ? dateTime.ToTimeZoned(dateFormat) : string.Empty;
        }

        public static string ToActiveToDateFormat(this DateTimeOffset dateTime, string dateFormat)
        {
            return dateTime.Date != DateTimeOffset.MaxValue.Date ? dateTime.ToDateZoned(dateFormat) : string.Empty;
        }

        public static string GetDateTimeDisplay(this DateTime date, string dateFormat, bool excludeTimePart = false)
        {
            string timePart = excludeTimePart ? string.Empty : $" {date.GetTimePart()}";
            return $"{date.ToString(dateFormat, CultureInfo.InvariantCulture)}{timePart}";
        }

        public static string GetDateTimeDisplay(this DateTimeOffset date, string dateFormat)
        {
            TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
            date.ToUniversalTime().ToOffset(timeSpan);
            var datePart = date.ToString(dateFormat, CultureInfo.InvariantCulture);
            var timePart = date.ToString("HH:mm");

            return $"{datePart} {timePart}";
        }

        public static DateTimeOffset? ConvertToOrganizationTimeZone(this DateTimeOffset? date)
        {
            if (date.HasValue) 
            {
                TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
                return date.Value.ToUniversalTime().ToOffset(timeSpan);
            }

            return date;
        }

        public static DateTimeOffset ConvertToOrganizationTimeZone(this DateTimeOffset date, string organizationTimeZone = null)
        {
            TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset(organizationTimeZone: organizationTimeZone));
            return date.ToUniversalTime().ToOffset(timeSpan);
        }

        public static string ConvertFormInstanceDateTimeToOrganizationTimeZone(this DateTimeOffset date)
        {
            TimeSpan timeSpan = TimeSpan.Parse(GlobalConfig.GetUserOffset());
            return date.ToOffset(timeSpan).ToString("yyyy-MM-ddTHH:mmzzz");
        }

        private static bool IsDateInUtcFormat(string datePart, out DateTime parsedDate)
        {
            return DateTime.TryParseExact(datePart, DateConstants.UTCDatePartFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }

        private static string GetTimezoneOffset(string timezoneOffset)
        {
            if (timezoneOffset == null)
            {
                timezoneOffset = GlobalConfig.GetUserOffset();
            }
            else
            {
                timezoneOffset = timezoneOffset.StartsWith('+') ? timezoneOffset.Substring(1) : timezoneOffset;
            }

            return timezoneOffset;
        }
    }
}