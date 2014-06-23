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

        private HttpClient _httpClient;
        private string _authenticationString;

        public Connector()
        {
            App.ExternalAction(x => x.StartProgress());

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppSettings.Current.Beta ? API_URL_BETA : API_URL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(PostRequest.JSON_MEDIA_TYPE));
        }

        public Task<Response<EmptyResponse>> Action<TRequest>(
                Func<Commands, Command> funcCommand,
                TRequest model)
            where TRequest : IRequestType
        {
            return this.Func<TRequest, EmptyResponse>(
                funcCommand,
                model);
        }


        public Task<Response<TResponse>> Func<TResponse>(
                Func<Commands, Command> funcCommand)
            where TResponse : class
        {
            return this.Func<EmptyRequest, TResponse>(
                funcCommand,
                new EmptyRequest());
        }

        public async Task<Response<TResponse>> Func<TRequest, TResponse>(
                Func<Commands, Command> funcCommand,
                TRequest model)
            where TRequest : IRequestType
            where TResponse : class
        {
            var request = new Request<TRequest>(model);
            var result = new Response<TResponse>();
            var command = funcCommand(Commands.Empty);
            try
            {
                if (request == null || request.Data == null)
                    return result.Invalid(ERROR_EMTPY_REQUEST);

                var validationResults = request.Validate();

                if (validationResults.Count() > 0)
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
                            response = await _httpClient.PostAsync(command.ToString(), content);
                        }
                        break;
                    case eRequestType.GET:
                        response = await _httpClient.GetAsync((command + Helper.GetUrlParams<TRequest>(request.Data)).ToString());
                        break;
                    case eRequestType.PUT:
                        using (var content = request.Data.GetContent())
                        {
                            response = await _httpClient.PutAsync(command.ToString(), content);
                        }
                        break;
                    case eRequestType.DELETE:
                        response = await _httpClient.DeleteAsync((command + Helper.GetUrlParams<TRequest>(request.Data)).ToString());
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
                    string resp_message = string.Empty;
                    if (response.Content != null)
                    {
                        resp_message = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        resp_message = response.ToString();
                    }
                    if (response.RequestMessage != null)
                        result = result.Fail(response.ReasonPhrase, resp_message, response.RequestMessage.ToString(), request.ToString());
                    else
                        result = result.Fail(response.ReasonPhrase, resp_message, request.ToString());
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
#if DEBUG
                result = result.Error(ex.Message, ex.ToString(), request.ToString());
#else
                result = result.Error(ERROR_INVALID_REQUEST, ex.Message, ex.ToString(), request.ToString());
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
