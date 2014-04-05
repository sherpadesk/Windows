using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SherpaDesk.Models.Response
{
    public abstract class Response : ObjectBase
    {
        protected Response()
        {
            this.Status = eResponseStatus.Success;
            this.Messages = new List<string>();
        }

        public eResponseStatus Status { get; set; }

        public IList<string> Messages { get; set; }

        public string Message
        {
            get
            {
                return this.Messages.FirstOrDefault() ?? string.Empty;
            }
        }
    }

    public sealed class Response<T> : Response
        where T : class
    {
        [Details]
        public T Result { get; set; }

        public async void Fill(HttpContent content)
        {
            this.Result = default(T);
            var jsonResponseSerializer = new DataContractJsonSerializer(typeof(T));
            using (var responseStream = await content.ReadAsStreamAsync())
            {
                responseStream.Position = 0;
                if (responseStream.Length > 0)
                {
                    try
                    {
                        this.Result = jsonResponseSerializer.ReadObject(responseStream) as T;
                    }
                    catch (Exception) { }
                }
            }
        }
    }

    [DataContract]
    public sealed class EmptyResponse { }

}
