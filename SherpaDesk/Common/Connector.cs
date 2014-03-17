﻿using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace SherpaDesk.Common
{
    public class Connector : IDisposable
    {
        protected const string API_URL = "http://api.sherpadesk.com/";
        protected const string ERROR_INVALID_REQUEST = "Unable to connect to sherpadesk.com";
        protected const string ERROR_EMTPY_REQUEST = "Request cannot be empty or null";
        protected const string ERROR_INVALID_RESPONSE = "Invalid response from sherpadesk.com";
        protected const string ERROR_INTERNAL = "internal error";
        protected const string JSON_MEDIA_TYPE = "application/json";
        protected const string AUTHORIZATION = "Authorization";
        protected const string BASIC = "Basic ";
        protected const string AUTH_FORMAT = "{0}-{1}:{2}";
        protected const string AUTH_FORMAT_ORG = "x:{0}";

        private HttpClient _httpClient;
        private string _authenticationString;

        public Connector()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(API_URL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));
        }

        public async Task<Response<TResponse>> Operation<TResponse>(
                string command)
            where TResponse : class
        {
            return await this.Operation<EmptyRequest, TResponse>(
                command,
                new EmptyRequest());
        }

        public async Task<Response<TResponse>> Operation<TRequest, TResponse>(
                string command,
                TRequest model)
            where TRequest : class
            where TResponse : class
        {
            var request = new Request<TRequest>(model);
            var result = new Response<TResponse>();
            try
            {
                if (request == null || request.Data == null)
                    return result.Invalid(ERROR_EMTPY_REQUEST);

                var validationResults = request.Validate();

                if (validationResults.Count > 0)
                    return result.Invalid(validationResults.Select(x => x.ErrorMessage).ToArray());

                var jsonRequestSerializer = new DataContractJsonSerializer(typeof(TRequest));

                string requestContent;
                using (var stream = new MemoryStream())
                {
                    jsonRequestSerializer.WriteObject(stream, model);

                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        requestContent = reader.ReadToEnd();
                    }
                }

                Authentication();

                HttpResponseMessage response;

                if (requestContent.Length > 3)
                {
                    response = await _httpClient.PostAsync(command,
                        new StringContent(requestContent, Encoding.UTF8, JSON_MEDIA_TYPE));
                }
                else
                {
                    response = await _httpClient.GetAsync(command);
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponseSerializer = new DataContractJsonSerializer(typeof(TResponse));
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        responseStream.Position = 0;
                        result.Result = (TResponse)jsonResponseSerializer.ReadObject(responseStream);
                    }
                }
                else
                {
                    return result.Fail(response.ReasonPhrase, response.ToString(), response.RequestMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                return result.Error(ERROR_INVALID_REQUEST, ex.Message, ex.ToString(), request.ToString());
            }
            return result;
        }


        public void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();
        }

        private void Authentication()
        {
            if (_httpClient.DefaultRequestHeaders.Contains(AUTHORIZATION))
            {
                _httpClient.DefaultRequestHeaders.Remove(AUTHORIZATION);
            }

            if (!string.IsNullOrEmpty(_authenticationString))
            {
                _httpClient.DefaultRequestHeaders.Add(AUTHORIZATION, _authenticationString);
            }
            else
            {
                var token = AppSettings.Current.ApiToken;
                if (!string.IsNullOrEmpty(token))
                {
                    var orgKey = AppSettings.Current.OrganizationKey;
                    var instKey = AppSettings.Current.InstanceKey;
                    if (!string.IsNullOrEmpty(orgKey) && !string.IsNullOrEmpty(instKey))
                    {
                        _authenticationString = 
                            Convert.ToBase64String(
                                Encoding.UTF8.GetBytes(
                                    string.Format(AUTH_FORMAT, orgKey, instKey, token)));

                        _httpClient.DefaultRequestHeaders.Add(AUTHORIZATION,
                            BASIC + _authenticationString);
                    }
                    else
                    {
                        _httpClient.DefaultRequestHeaders.Add(AUTHORIZATION,
                            BASIC + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(AUTH_FORMAT_ORG, token))));
                    }
                }
            }

        }
    }
}
