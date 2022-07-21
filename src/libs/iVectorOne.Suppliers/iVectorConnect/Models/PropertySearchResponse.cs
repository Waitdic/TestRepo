namespace iVectorOne.CSSuppliers.iVectorConnect.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using iVectorOne.CSSuppliers.iVectorConnect.Models.Common;

    [XmlRoot("PropertySearchResponse")]
    public class PropertySearchResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        [XmlArray("PropertyResults")]
        [XmlArrayItem("PropertyResult")]
        public List<PropertyResult> PropertyResults { get; set; } = new();
    }
}
