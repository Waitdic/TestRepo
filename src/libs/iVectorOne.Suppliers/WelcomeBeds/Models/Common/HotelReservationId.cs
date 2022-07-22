namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class HotelReservationId
    {
        public HotelReservationId() { }

        [XmlAttribute("ResID_Value")]
        public string ResIdValue { get; set; } = string.Empty;
    }
}
