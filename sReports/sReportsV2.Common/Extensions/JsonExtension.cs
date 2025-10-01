using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Web;

namespace sReportsV2.Common.Extensions
{
    public static class JsonExtension
    {
        public static string ToJsonUrlEncoded(this object entity)
        {
            entity = Ensure.IsNotNull(entity, nameof(entity));
            try
            {
                return HttpUtility.UrlEncode(entity.JsonSerialize());
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string JsonSerialize(this object entity, bool lowerCamelCase = false, bool includeNull = false)
        {
            try
            {
                JsonSerializerSettings settings = null;
                if (lowerCamelCase)
                {
                    settings = new JsonSerializerSettings()
                    {
                        NullValueHandling = includeNull ? NullValueHandling.Include : NullValueHandling.Ignore,
                        Formatting = includeNull ? Formatting.None : Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                }
                return JsonConvert.SerializeObject(entity, settings);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
