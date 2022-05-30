namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.iVectorConnect.Models.Common;

    [XmlRoot("PropertySearchResponse")]
    public class PropertySearchResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        [XmlArray("PropertyResults")]
        [XmlArrayItem("PropertyResult")]
        public List<PropertyResult> PropertyResults { get; set; } = new();
    }
}
