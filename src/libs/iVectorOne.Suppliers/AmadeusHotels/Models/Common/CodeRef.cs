namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class CodeRef
    {
        [XmlAttribute]
        public string CodeContext { get; set; } = string.Empty;

        [XmlAttribute]
        public string LocationCode { get; set; } = string.Empty;
    }
}