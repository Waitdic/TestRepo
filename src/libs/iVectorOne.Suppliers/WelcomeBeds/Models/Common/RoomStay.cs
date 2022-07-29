namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomStay
    {
        public RoomStay() { }

        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();

        [XmlArray("RatePlans")]
        [XmlArrayItem("RatePlan")]
        public List<RatePlan> RatePlans { get; set; } = new List<RatePlan>();

        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public List<RoomRate> RoomRates { get; set; } = new List<RoomRate>();

        [XmlElement("TimeSpan")]
        public StayTimeSpan TimeSpan { get; set; } = new StayTimeSpan();

        [XmlElement("BasicPropertyInfo")]
        public BasicPropertyInfo BasicPropertyInfo { get; set; } = new BasicPropertyInfo();

        [XmlElement("TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new TpaExtensions();

        [XmlElement("Total")]
        public RateTotal Total { get; set; } = new RateTotal();
    }

}
