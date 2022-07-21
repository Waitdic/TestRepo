namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using System;
    using System.Xml.Serialization;

    public class HotelResponse
    {
        [XmlElement("Envelope")]
        public Envelope<HotelSearchWithRoomsResponse>[] Envelopes { get; set; } = Array.Empty<Envelope<HotelSearchWithRoomsResponse>>();
    }
}
