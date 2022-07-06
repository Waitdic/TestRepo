namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ResGlobalInfo
    {
        [XmlArray("ResIDs")]
        [XmlArrayItem("ResID")]
        public List<ResID> ResIDs { get; set; } = new();
    }
}
