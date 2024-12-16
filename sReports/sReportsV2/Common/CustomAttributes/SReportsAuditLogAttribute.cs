using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Mongo;
using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class SReportsAuditLogAttribute : ActionFilterAttribute
    {
        private readonly string[] excludeParamList;

        public SReportsAuditLogAttribute()
        {
            this.excludeParamList = Array.Empty<string>();
        }

        public SReportsAuditLogAttribute(string[] excludeParamList)
        {
            this.excludeParamList = excludeParamList;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));
            var al = new AuditLog
            {
                Action = filterContext.ActionDescriptor.RouteValues["action"],
                Controller = filterContext.ActionDescriptor.RouteValues["controller"],
                Username = filterContext.HttpContext.User.Identity.Name,
                Time = DateTime.Now,
                Json = JsonConvert.SerializeObject(PrepareParametersBeforeSerialization(filterContext.ActionArguments))
            };

            Task.Run(() => SaveLog(al)); //fire and forget*/
        }

        private void SaveLog(AuditLog log)
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            IMongoCollection<AuditLog> Collection = MongoDatabase.GetCollection<AuditLog>(MongoCollectionNames.AuditLog);
            Collection.InsertOne(log);
        }

        #region Mask Sensitive Data

        private IDictionary<string, object> PrepareParametersBeforeSerialization(IDictionary<string, object> actionParameters)
        {
            IDictionary<string, object> maskedValues = ObjectCloneExtensions.Copy(actionParameters);

            foreach (var paramName in actionParameters.Keys)
            {
                var paramValue = maskedValues[paramName];
                if (paramValue != null)
                {
                    var type = paramValue.GetType();

                    if (ParamContainsSensitiveData(paramName))
                    {
                        maskedValues[paramName] = GetSensitiveValue(type);
                    }
                    else if (!IsSimple(type))
                    {
                        maskedValues[paramName] = PrepareComplexParameterBeforeSerialization(paramValue);
                    }
                }
            }
            return maskedValues;
        }

        private bool ParamContainsSensitiveData(string paramName)
        {
            return excludeParamList.Contains(paramName);
        }

        private object GetSensitiveValue(Type type)
        {
            if (type == typeof(string))
            {
                return GetSensitiveReplacement();
            } 
            else
            {
                return GetDefaultValueBasedOnType(type);
            }
        }

        private object GetDefaultValueBasedOnType(Type propertyType)
        {
            if (propertyType.GetCustomAttributes(typeof(DefaultValueAttribute), true)
                .FirstOrDefault() is DefaultValueAttribute defaultValueAttribute)
            {
                return defaultValueAttribute.Value;
            }
            
            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }

        private object PrepareComplexParameterBeforeSerialization(object obj)
        {
            if (obj == null) return obj;

            if (obj is IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = PrepareComplexParameterBeforeSerialization(list[i]);
                }
            }
            else if (obj is IDictionary dictionary)
            {
                foreach (var item in dictionary.Keys.Copy())
                {
                    dictionary[item] = PrepareComplexParameterBeforeSerialization(dictionary[item]);
                }
            }
            else if (obj.GetType().IsClass)
            {
                obj = TransformObjectIfNecessarry(obj);
                PrepareObjectParameterBeforeSerialization(obj);
            }

            return obj;
        }

        private object TransformObjectIfNecessarry(object obj)
        {
            if (obj is IFormFile formFile)
            {
                obj = new CustomFormFile(formFile);
            }
            return obj;
        }

        private void PrepareObjectParameterBeforeSerialization(object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.CanWrite)
                {
                    Type propType = prop.PropertyType;
                    if (ParamContainsSensitiveData(prop.Name))
                    {
                        prop.SetValue(obj, GetSensitiveValue(propType));
                    }
                    else if (!IsSimple(propType))
                    {
                        prop.SetValue(obj,
                            PrepareComplexParameterBeforeSerialization(prop.GetValue(obj))
                        );
                    }
                }
            }
        }

        private bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }

        private string GetSensitiveReplacement()
        {
            return new string(new char[] { '*' });
        }

        #endregion /Mask Sensitive Data
    }
}