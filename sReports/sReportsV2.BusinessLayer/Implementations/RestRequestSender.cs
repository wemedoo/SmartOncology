using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ApiRequest;
using System;
using System.Collections.Generic;
using System.Net;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class RestRequestSender : IRestRequestSender
    {
        public ApiRequestLog ApiRequestLog { get; set; }
        private readonly SReportsContext dbContext;

        public RestRequestSender(SReportsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public RestResponse GetResponse(RestRequestData restRequestData, IAuthenticator authenticator = null)
        {
            try
            {
                RestClient client = new RestClient(restRequestData.BaseUrl);

                RestRequest request = new RestRequest(restRequestData.Endpoint, Method.Post);
                request.Timeout = TimeSpan.FromMinutes(10);
                if (authenticator != null)
                {
                    request.Authenticator = authenticator;
                }
                request.AddJsonBody(restRequestData.Body);

                return Execute(client, request, restRequestData);
            }
            catch (System.Exception ex)
            {
                LogHelper.Error($"Error while sending request to external API: {restRequestData.BaseUrl}/{restRequestData.Endpoint}, error: {ex.Message}");
                return null;
            }
        }

        private void HandleRequestBeforeExecution(RestRequestData restRequestData)
        {
            string requestBodySerialized = JsonConvert.SerializeObject(restRequestData.Body);
            LogHelper.Info($"Sent request to {restRequestData.ApiName}: {requestBodySerialized}");
            ApiRequestLog = new ApiRequestLog(restRequestData, Common.Enums.ApiRequestDirection.Outgoing, requestBodySerialized);
        }

        private void SetHeader(RestRequest request, IDictionary<string, string> headerParameters)
        {
            if (headerParameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in headerParameters)
                {
                    request.AddHeader(parameter.Key, parameter.Value);
                }
            }
        }

        private RestResponse Execute(RestClient client, RestRequest request, RestRequestData restRequestData)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            SetHeader(request, restRequestData.HeaderParameters);
            HandleRequestBeforeExecution(restRequestData);
            RestResponse restResponse = client.Execute(request);
            HandleResponseAfterExecution(restResponse);

            return restResponse;
        }

        private void HandleResponseAfterExecution(RestResponse restResponse)
        {
            if ((int)restResponse.StatusCode >= 500 || !string.IsNullOrEmpty(restResponse.ErrorMessage))
            {
                LogHelper.Error($"Status code: {restResponse.StatusCode}, Response content: {restResponse.Content}, Response uri: {restResponse.ResponseUri}, error message: {restResponse.ErrorMessage}");
            }
            else
            {
                LogHelper.Info($"Status code: {restResponse.StatusCode}, Response content: {restResponse.Content}, Response uri: {restResponse.ResponseUri}");
            }
            SaveApiRequestLog(restResponse);
        }

        private void SaveApiRequestLog(RestResponse restResponse)
        {
            ApiRequestLog.LogResponseData(restResponse.StatusCode, restResponse.Content);
            dbContext.ApiRequestLogs.Add(ApiRequestLog);
            dbContext.SaveChanges();
        }
    }
}
