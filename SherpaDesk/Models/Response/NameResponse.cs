using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class NameResponse : ObjectBase
    {

        [DataMember(Name = "id"), Details]
        public int Id { get; set; }

        [DataMember(Name = "name"), Details]
        public string Name { get; set; }
    }
}
