using SherpaDesk.Common;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class DeleteRequest : PutRequest
    {
        public DeleteRequest(string key)
            : base(key)
        {

        }

        [Details]
        public override eRequestType Type
        {
            get { return eRequestType.DELETE; }
        }

    }
}
