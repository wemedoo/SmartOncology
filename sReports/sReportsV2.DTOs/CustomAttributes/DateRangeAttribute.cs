using sReportsV2.DTOs.Common.DTO;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class DateRangeAttribute : ValidationAttribute 
    {
        public string GetErrorMessage() => $"Period end date cannot be before Period start date.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if(!DateRangeValid(validationContext.ObjectInstance, value))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        private bool DateRangeValid(object objectInstance, object endDateTimeValue)
        {
            if (objectInstance is PeriodDTO period)
            {
                return period.StartDate <= (DateTime)endDateTimeValue;
            }
            else if (objectInstance is PeriodOffsetDTO periodOffset)
            {
                return periodOffset.StartDate <= (DateTimeOffset)endDateTimeValue;
            }
            else
            {
                return false;
            }
        }
    }
}
