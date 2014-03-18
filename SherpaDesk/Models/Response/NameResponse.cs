using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
