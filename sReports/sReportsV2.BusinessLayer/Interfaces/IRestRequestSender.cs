using RestSharp;
using RestSharp.Authenticators;
using sReportsV2.Common.Entities;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IRestRequestSender
    {
        RestResponse GetResponse(RestRequestData restRequestData, IAuthenticator authenticator = null);
    }
}
