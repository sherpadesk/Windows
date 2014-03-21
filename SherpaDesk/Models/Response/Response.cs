using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
    }

    [DataContract]
    public sealed class EmptyResponse { }

}
