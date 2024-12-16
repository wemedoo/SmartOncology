using Microsoft.AspNetCore.Http;
using sReportsV2.Common.Extensions;
using System;

namespace sReportsV2.Common.Helpers
{
    public static class UrlHelper
    {
        public static string GetBaseUrl(HttpRequest req)
        {
            req = Ensure.IsNotNull(req, nameof(req));
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }

        public static string GetPreviewThesaurusUrlTemplate(this string host)
        {
            return $"{host}ThesaurusEntry/EditByO4MtId?id=";
        }
    }
}
