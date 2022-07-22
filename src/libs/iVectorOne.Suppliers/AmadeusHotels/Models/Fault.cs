namespace iVectorOne.CSSuppliers.AmadeusHotels.Models
{
    using System.Xml.Serialization;
    using Soap;

    public class Fault : SoapContent
    {
        [XmlElement("faultstring")]
        public string FaultString { get; set; } = string.Empty;
    }
}
