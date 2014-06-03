using SherpaDesk.Common;
using System.Net.Http;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{

    [DataContract]
    public abstract class GetRequest : ObjectBase, IRequestType
    {
        [Details]
        public virtual eRequestType Type
        {
            get { return eRequestType.GET; }
        }

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public HttpContent GetContent()
        {
            throw new System.NotImplementedException("GET request does not support content.");
        }
    }
}
