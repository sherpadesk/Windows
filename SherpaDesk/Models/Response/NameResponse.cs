using SherpaDesk.Common;
using System.Runtime.Serialization;
using SherpaDesk.Interfaces;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class NameResponse : ObjectBase, IKeyName
    {
        public static IKeyName Empty = new NameResponse() { Id = 0, Name = string.Empty };

        [DataMember(Name = "id"), Details]
        public int Id { get; set; }

        [DataMember(Name = "name"), Details]
        public string Name { get; set; }

        public object Key
        {
            get { return this.Id; }
        }
    }
}
