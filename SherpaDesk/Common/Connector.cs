using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


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

        private HttpClient _httpClient;
        private string _authenticationString;

        public Connector()
        {
            if (Window.Current.Content is Frame &&
                ((Frame)Window.Current.Content).Content is MainPage)
                ((MainPage)((Frame)Window.Current.Content).Content).StartProgress();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppSettings.Current.Beta ? API_URL_BETA : API_URL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(PostRequest.JSON_MEDIA_TYPE));
        }

        public Task<Response<EmptyResponse>> Action<TRequest>(
                string command,
                TRequest model)
            where TRequest : IRequestType
        {
            return this.Func<TRequest, EmptyResponse>(
                command,
                model);
        }


        public Task<Response<TResponse>> Func<TResponse>(
                string command)
            where TResponse : class
        {
            return this.Func<EmptyRequest, TResponse>(
                command,
                new EmptyRequest());
        }

        public async Task<Response<TResponse>> Func<TRequest, TResponse>(
                string command,
                TRequest model)
            where TRequest : IRequestType
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

                Authentication();

                HttpResponseMessage response = null;

                if (model is IPath)
                {
                    command += ((IPath)model).Path;
                }

                switch (request.Data.Type)
                {
                    case eRequestType.POST:
                        using (var content = request.Data.GetContent())
                        {
                            response = await _httpClient.PostAsync(command, content);
                        }
                        break;
                    case eRequestType.GET:
                        response = await _httpClient.GetAsync(command + Helper.GetUrlParams<TRequest>(request.Data));
                        break;
                    case eRequestType.PUT:
                        using (var content = request.Data.GetContent())
                        {
                            response = await _httpClient.PutAsync(command, content);
                        }
                        break;
                    case eRequestType.DELETE:
                        response = await _httpClient.DeleteAsync(command + Helper.GetUrlParams<TRequest>(request.Data));
                        break;
                    default:
                        throw new ArgumentException("Unknown type of request!");
                }
                if (response.IsSuccessStatusCode)
                {
                    result.Fill(response.Content);
                }
                else
                {
                    result = result.Fail(response.ReasonPhrase, response.ToString(), response.RequestMessage != null ? response.RequestMessage.ToString() : string.Empty);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                result = result.Error(ERROR_INVALID_REQUEST, ex.Message, ex.ToString(), request.ToString());
            }
            return result;
        }

        public void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();
            if (Window.Current.Content is Frame &&
                ((Frame)Window.Current.Content).Content is MainPage)
                ((MainPage)((Frame)Window.Current.Content).Content).StopProgress();
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
