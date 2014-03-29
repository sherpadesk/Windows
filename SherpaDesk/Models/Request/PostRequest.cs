using SherpaDesk.Common;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public abstract class PostRequest : ObjectBase, IRequestType
    {
        public const string JSON_MEDIA_TYPE = "application/json";

        [Details]
        public virtual eRequestType Type
        {
            get { return eRequestType.POST; }
        }

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public virtual HttpContent GetContent()
        {
            var jsonRequestSerializer = new DataContractJsonSerializer(this.GetType());

            string requestContent;
            using (var stream = new MemoryStream())
            {
                jsonRequestSerializer.WriteObject(stream, this);

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    requestContent = reader.ReadToEnd();
                }
            }
            var content = new StringContent(requestContent, Encoding.UTF8, JSON_MEDIA_TYPE);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE);
            return content;
        }
    }
}
