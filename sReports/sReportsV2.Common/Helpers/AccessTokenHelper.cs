using System;
using System.Linq;

namespace sReportsV2.Common.Helpers
{
    public static class AccessTokenHelper
    {
        private static string accessToken;
        public static string GetAccessToken()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                //byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                //byte[] guid = Guid.NewGuid().ToByteArray();
                //accessToken = Convert.ToBase64String(time.Concat(guid).ToArray());
            }
            accessToken = "uVZRY0/y2kivWyRQvZx9SZCrvBvG04op";
            return accessToken;
        }
    }
}
