using SherpaDesk.Common;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public abstract class PutRequest : PostRequest, IPath
    {
        public PutRequest(string key)
        {
            this.Path = "/" + key;
        }

        [Details]
        public string Path { get; set; }

        [Details]
        public override eRequestType Type
        {
            get { return eRequestType.PUT; }
        }
    }
}
