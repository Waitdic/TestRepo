namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class BookResponse
    {
        public BookResponse() { }

        [XmlElement("BookingConfirmation")]
        public BookingConfirmation BookingConfirmation { get; set; } = new();
    }
}
