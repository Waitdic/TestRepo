namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class BookingDetail
    {
        [XmlAttribute]
        public string BookingId { get; set; } = string.Empty;

        public HotelCancelPolicies HotelCancelPolicies { get; set; } = new();
    }
}