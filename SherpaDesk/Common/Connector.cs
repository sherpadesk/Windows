using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SherpaDesk.Extensions;
using SherpaDesk.Interfaces;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace SherpaDesk.Common
{
    public class Connector : IDisposable
    {
        public const string API_URL = "https://api.sherpadesk.com/";
        public const string API_URL_BETA = "http://api.beta.sherpadesk.com/";
        public const string ERROR_INVALID_REQUEST = "Unable to connect to sherpadesk.com";
        protected const string ERROR_EMTPY_REQUEST = "Request cannot be empty or null";
        protected const string ERROR_INVALID_RESPONSE = "Invalid response from sherpadesk.com";
        protected const string ERROR_INTERNAL = "internal error";
        protected const string AUTHORIZATION = "Authorization";
        protected const string BASIC = "Basic ";
        protected const string AUTH_FORMAT = "{0}-{1}:{2}";
        protected const string AUTH_FORMAT_ORG = "x:{0}";

        private readonly HttpClient _httpClient;
        private string _authenticationString;

        public Connector()
        {
            App.ExternalAction(x => x.StartProgress());

            _httpClient = new HttpClient { BaseAddress = new Uri(AppSettings.Current.Beta ? API_URL_BETA : API_URL) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(PostRequest.JSON_MEDIA_TYPE));
        }

        public Task<Response<EmptyResponse>> Action<TRequest>(
                Func<Commands, Command> funcCommand,
                TRequest model)
            where TRequest : class, IRequestType
        {
            return Func<TRequest, EmptyResponse>(
                funcCommand,
                model);
        }


        public Task<Response<TResponse>> Func<TResponse>(
                Func<Commands, Command> funcCommand)
            where TResponse : class
        {
            return Func<EmptyRequest, TResponse>(
                funcCommand,
                new EmptyRequest());
        }

        public async Task<Response<TResponse>> Func<TRequest, TResponse>(
                Func<Commands, Command> funcCommand,
                TRequest model)
            where TRequest : class, IRequestType
            where TResponse : class
        {
            var request = new Request<TRequest>(model);
            var result = new Response<TResponse>();
            var command = funcCommand(Commands.Empty);
            try
            {
                if (request.Data == null)
                    return result.Invalid(ERROR_EMTPY_REQUEST);

                var validationResults = request.Validate();

                var validationResultList = validationResults as IList<ValidationResult> ?? validationResults.ToList();
                if (validationResultList.Any())
                    return result.Invalid(validationResultList.Select(x => x.ErrorMessage).ToArray());

                Authentication();

                HttpResponseMessage response;

                var path = model as IPath;
                if (path != null)
                {
                    command += path.Path;
                }

                await App.WriteLog(command.ToString() + ": " + request.ToString(), eErrorType.Message);

                //var req_message = await request.Data.GetContent().ReadAsStringAsync();
                switch (request.Data.Type)
                {
                    case eRequestType.POST:
                        using (var content = request.Data.GetContent())
                        {
                            response = await _httpClient.PostAsync(command.ToString(), content);
                        }
                        break;
                    case eRequestType.GET:
                        response = await _httpClient.GetAsync((command + Helper.GetUrlParams(request.Data)).ToString());
                        break;
                    case eRequestType.PUT:
                        using (var content = request.Data.GetContent())
                        {
                            response = await _httpClient.PutAsync(command.ToString(), content);
                        }
                        break;
                    case eRequestType.DELETE:
                        response = await _httpClient.DeleteAsync((command + Helper.GetUrlParams(request.Data)).ToString());
                        break;
                    default:
                        throw new ArgumentException("Unknown type of request!");
                }
                if (response.IsSuccessStatusCode)
                {
//                    var resp_message = await response.Content.ReadAsStringAsync();
                    result.Fill(response.Content);
                }
                else
                {
                    string resp_message;
                    if (response.Content != null)
                    {
                        resp_message = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        resp_message = response.ToString();
                    }
                    result = response.RequestMessage != null ? 
                        result.Fail(response.ReasonPhrase, resp_message, response.RequestMessage.ToString(), request.ToString()) : 
                        result.Fail(response.ReasonPhrase, resp_message, request.ToString());
                }
                await App.WriteLog(result.ToString(), eErrorType.Message);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
#if DEBUG
                result = result.Error(message, ex.ToString(), request.ToString());
#else
                result = result.Error(ERROR_INVALID_REQUEST, message, ex.ToString(), request.ToString());
#endif
            }
            return result;
        }

        public void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();

            App.ExternalAction(x => x.StopProgress());
        }

        private void Authentication()
        {
            if (_httpClient.DefaultRequestHeaders.Contains(AUTHORIZATION))
            {
                _httpClient.DefaultRequestHeaders.Remove(AUTHORIZATION);
            }

            if (!string.IsNullOrEmpty(_authenticationString))
            {
                _httpClient.DefaultRequestHeaders.Add(AUTHORIZATION, BASIC + _authenticationString);
            }
            else
            {
                var token = AppSettings.Current.ApiToken;

                if (string.IsNullOrEmpty(token)) return;

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
