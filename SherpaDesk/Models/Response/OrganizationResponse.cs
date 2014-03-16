using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Response
{
    [DataContract] 
    public class InstanceResponse : ObjectBase
    {
        [DataMember(Name = "key"), Details("********")]
        public string Key { get; set; }

        [DataMember(Name = "name"), Details]
        public string Name { get; set; }
    }

    [DataContract]
    public class OrganizationResponse : InstanceResponse
    {
        [DataMember(Name = "instances"), Details]
        public InstanceResponse[] Instances { get; set; }

    }
}
