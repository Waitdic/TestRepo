namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class FreetextData
    {
        [XmlElement("freetextDetail")]
        public FreetextDetail FreetextDetail { get; set; } = new();

        [XmlElement("longFreetext")]
        public string LongFreetext { get; set; } = string.Empty;
    }
}
