using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace sReportsV2.DTOs.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class ObjectPropertiesRequiredAttribute : ValidationAttribute
    {
        private string[] _propertyNames { get; set; }
        private string[] _propertyDisplayNames { get; set; }

        public ObjectPropertiesRequiredAttribute(string[] propertyNames, string[] propertyDisplayNames)
        {
            if (propertyNames.Length != propertyDisplayNames.Length) throw new ArgumentException("PropertyNames and PropertyDisplayNames should have equal length");
            _propertyNames = propertyNames;
            _propertyDisplayNames = propertyDisplayNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> validationMessages = new List<string>();
            if (value != null)
            {
                for (int index = 0; index < _propertyNames.Length; ++index)
                {
                    PropertyInfo property = value.GetType().GetProperty(_propertyNames[index]);
                    if (property != null)
                    {
                        object propertyValue = property.GetValue(value);
                        if (propertyValue == null)
                        {
                            validationMessages.Add($"Field {_propertyDisplayNames[index]} is required.");
                        }
                    }
                }
            }

            return validationMessages.Count == 0 ? ValidationResult.Success : new ValidationResult(string.Join(", ", validationMessages));
        }
    }
}
