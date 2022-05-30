namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class PosSource
    {
        public PosSource() { }

        [XmlElement("BookingChannel")]
        public BookingChannel BookingChannel { get; set; } = new();
    }
}
