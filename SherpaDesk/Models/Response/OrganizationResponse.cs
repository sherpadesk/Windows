using SherpaDesk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using SherpaDesk.Interfaces;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class InstanceResponse : ObjectBase, IKeyName
    {
        [DataMember(Name = "key"), Details("********")]
        public string Key { get; set; }

        [DataMember(Name = "name"), Details]
        public string Name { get; set; }

        object IKeyName.Key
        {
            get { return this.Key; }
        }
    }

    [DataContract]
    public class OrganizationResponse : InstanceResponse
    {
        [DataMember(Name = "instances"), Details]
        public InstanceResponse[] Instances { get; set; }
    }
}
