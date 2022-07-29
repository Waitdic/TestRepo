namespace iVectorOne.Suppliers.MTS.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [System.Serializable()]
    [XmlRoot("OTA_HotelAvailRS", IsNullable = false, Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSSearchResponse
    {
        [XmlArray("HotelStays")]
        public List<Hotel> Hotels = new();

        [XmlArray("RoomStays")]
        public List<Room> Rooms = new();

        [XmlArray()]
        public List<Area> Areas = new();

        [XmlType("HotelStay")]
        public class Hotel
        {
            [XmlElement("BasicPropertyInfo")]
            public HotelInfo Info = new();
        }

        public class HotelInfo
        {
            [XmlAttribute()]
            public string HotelCode = string.Empty;
            [XmlAttribute()]
            public string AreaID = string.Empty;
        }

        [XmlType("RoomStay")]
        public class Room
        {
            [XmlAttribute("RoomStayCandidateRPH")]
            public int ID;

            [XmlElement("BasicPropertyInfo")]
            public HotelInfo Info = new();

            [XmlArray()]
            public List<RoomRate> RoomRates = new();

            [XmlArray()]
            public List<RoomType> RoomTypes = new();

        }

        [XmlType()]
        public class RoomRate
        {
            [XmlArray()]
            public List<Rate> Rates = new();

            [XmlArray()]
            public List<Feature> Features = new();

            [XmlAttribute()]
            public int NumberOfUnits;

            [XmlAttribute()]
            public string RoomTypeCode = string.Empty;
        }

        public class Rate
        {
            [XmlElement()]
            public TotalDetails Total = new();
        }

        public class TotalDetails
        {
            [XmlAttribute()]
            public decimal AmountAfterTax;
            [XmlAttribute()]
            public string CurrencyCode = string.Empty;
        }

        public class Feature
        {
            [XmlElement("Description")]
            public List<Description> Descriptions = new();
        }

        public class Description
        {
            [XmlElement()]
            public string Text = string.Empty;
            [XmlAttribute()]
            public string Name = string.Empty;
        }

        public class RoomType
        {
            [XmlElement()]
            public Description RoomDescription = new();

            [XmlAttribute("RoomTypeCode")]
            public string Code = string.Empty;
        }

        public class Area
        {
            [XmlAttribute()]
            public string AreaID = string.Empty;

            [XmlElement("AreaDescription")]
            public List<Description> Descriptions = new();
        }
    }
}