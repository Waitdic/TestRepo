namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Reservation
    {
        [XmlElement("controlNumber")]
        public string ControlNumber { get; set; } = string.Empty;
    }
}
