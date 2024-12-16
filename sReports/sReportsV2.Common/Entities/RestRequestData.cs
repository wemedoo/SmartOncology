using System.Collections.Generic;

namespace sReportsV2.Common.Entities
{
    public class RestRequestData
    {
        public object Body { get; set; }
        public string BaseUrl { get; set; }
        public string Endpoint { get; set; }
        public string ApiName { get; set; }
        public IDictionary<string, string> HeaderParameters { get; set; }
    }
}
