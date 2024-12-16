using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using sReportsV2.DTOs.User.DTO;

namespace sReportsV2.Common.Extensions
{
    public static class SessionExtensions
    {
        public static UserCookieData GetUserFromSession(this ISession session)
        {
            Ensure.IsNotNull(session, nameof(session));

            if (!session.TryGetValue("userData", out byte[] userDataBytes))
            {
                // Session data not found, handle accordingly.
                return null;
            }

            string userDataJson = System.Text.Encoding.UTF8.GetString(userDataBytes);
            var userCookieData = JsonConvert.DeserializeObject<UserCookieData>(userDataJson);

            return userCookieData;
        }

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}