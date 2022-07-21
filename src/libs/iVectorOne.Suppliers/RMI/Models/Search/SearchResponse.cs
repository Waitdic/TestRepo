using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.RMI.Models
{
    [XmlRoot("SearchResponse")]
    public class SearchResponse
    {
        [XmlElement("ReturnStatus")]
        public ReturnStatus ReturnStatus { get; set; } = new();
        [XmlArray("PropertyResults")]
        [XmlArrayItem("PropertyResult")]
        public List<PropertyResult> PropertyResults { get; set; } = new();
    }
}
