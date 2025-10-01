using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace sReportsV2.Common.Extensions
{
    public static class ObjectExtension
    {
        public static string GetDataAttr(this object data)
        {
            data = Ensure.IsNotNull(data, nameof(data));
            StringBuilder stringBuilder = new StringBuilder();
            List<PropertyInfo> properties = data.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(DataPropAttribute))).ToList();
            properties.ForEach(x =>
            {

                object value = x.GetValue(data, null);
                string appendValue = string.Empty;
                if (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string) || x.PropertyType.IsEnum)
                {
                    appendValue = System.Net.WebUtility.UrlEncode(value?.ToString());
                }else if(x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?))
                {
                    var valueDateTime = value as DateTime?;
                    if(valueDateTime != null)
                    {
                        appendValue = System.Net.WebUtility.UrlEncode(valueDateTime.Value.ToString("o"));

                    }

                }
                else
                {
                    appendValue = System.Net.WebUtility.UrlEncode(value.JsonSerialize(true, true));
                }
                
                if(string.IsNullOrWhiteSpace(appendValue))
                {
                    stringBuilder.AppendLine($"data-{x.Name.ToLower()}=\"\" ");
                }
                else
                {
                    stringBuilder.AppendLine($"data-{x.Name.ToLower()}={appendValue } ");
                }
            });

            string result = stringBuilder.ToString();

            return result;
        }

        public static List<string> GetIdsFromObject(this object formElement)
        {
            formElement = Ensure.IsNotNull(formElement, nameof(formElement));

            List<string> ids = new List<string>();
            foreach (PropertyInfo prop in formElement.GetType().GetProperties())
            {
                if (prop.Name == "Id")
                {
                    ids.Add(prop.GetValue(formElement).ToString());
                }
                else
                {
                    var value = prop.GetValue(formElement);
                    if (value != null && (value is List<FormChapter> || value is List<FormPage> || value is List<Field> || value is List<FormFieldValue>))
                    {
                        foreach (var o in ((IEnumerable<object>)value).ToList())
                        {
                            ids.AddRange(o.GetIdsFromObject());
                        }
                    }
                    else if (value is List<List<FieldSet>>)
                    {
                        foreach (var list in ((IEnumerable<object>)value).ToList())
                        {
                            foreach (var ob in (IEnumerable<object>)list)
                            {
                                ids.AddRange(ob.GetIdsFromObject());
                            }
                        }
                    }
                }
            }

            return ids;
        }

        public static string GetAttr(this Dictionary<object, object> htmlAttributes)
        {
            htmlAttributes = Ensure.IsNotNull(htmlAttributes, nameof(htmlAttributes));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<object, object> attribute in htmlAttributes)
            {
                if (attribute.Value != null)
                {
                    stringBuilder.Append($"{attribute.Key}=\"{attribute.Value}\" ");
                }
                else
                {
                    stringBuilder.Append($"{attribute.Key} ");
                }
            }
            
            return stringBuilder.ToString();
        }

        public static List<CodeDataOut> FilterDataSource(this List<CodeDataOut> allCodes, bool? readOnlyMode = null, int? selectedCodeId = null)
        {
            allCodes = Ensure.IsNotNull(allCodes, nameof(allCodes));
            if (readOnlyMode == true)
            {
                return selectedCodeId.HasValue ? allCodes.Where(c => c.Id == selectedCodeId).ToList() : allCodes;
            } 
            else
            {
                return allCodes.Where(c => c.IsActive()).ToList();
            }
        }

        public static bool IsLockOrUnlockAction(this FormState formState)
        {
            return formState == FormState.Locked || formState == FormState.Unlocked;
        }
    }
}