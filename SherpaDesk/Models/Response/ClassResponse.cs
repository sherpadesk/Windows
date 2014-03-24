using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class ClassResponse : NameResponse
    {
        [DataMember(Name = "parent_id"), Details]
        public int ParentId { get; set; }

        [DataMember(Name = "hierarchy_level"), Details]
        public int HierarchyLevel { get; set; }

        //"sub\":null

        [DataMember(Name = "is_lastchild"), Details]
        public bool LastChild { get; set; }

        [DataMember(Name = "is_restrict_to_techs"), Details]
        public bool RestrictToTechs { get; set; }

        [DataMember(Name = "is_active"), Details]
        public bool Active { get; set; }
    }

}
