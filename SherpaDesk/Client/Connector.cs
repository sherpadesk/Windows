using SherpaDesk.Models;
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


namespace SherpaDesk.Client
{
    public class Connector : IDisposable
    {
        protected const string API_URL = "http://api.sherpadesk.com/";
        protected const string ERROR_INVALID_REQUEST = "Unable to connect to sherpadesk.com";
        protected const string ERROR_EMTPY_REQUEST = "Request cannot be empty or null";
        protected const string ERROR_INVALID_RESPONSE = "Invalid response from sherpadesk.com";
        protected const string ERROR_INTERNAL = "internal error";
        protected const string JSON_MEDIA_TYPE = "application/json";

        private HttpClient _httpClient;

        public Connector()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(API_URL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));
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
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        var jsonResponseSerializer = new DataContractJsonSerializer(typeof(TResponse));

                        result.Data = jsonResponseSerializer.ReadObject(responseStream) as TResponse;
                        if (result.Data == null)
                            return result.Fail(ERROR_INVALID_RESPONSE, (new StreamReader(responseStream)).ReadToEnd());
                    }
                }
                else
                {
                    return result.Fail(response.ReasonPhrase, response.ToString(), response.RequestMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                return result.Error(ERROR_INVALID_REQUEST, ex.Message, ex.ToString());
            }
            return result;
        }


        public void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();
        }
    }
}
