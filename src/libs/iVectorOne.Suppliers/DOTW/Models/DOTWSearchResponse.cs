namespace iVectorOne.Suppliers.DOTW.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [System.Serializable()]
    [XmlRoot("result", IsNullable = false)]
    public class DOTWSearchResponse
    {
        [XmlArray("hotels")]
        public List<Hotel> Hotels = new List<Hotel>();

        [XmlType("hotel")]
        public class Hotel
        {
            [XmlAttribute("hotelid")]
            public string HotelID = string.Empty;
            [XmlArray("rooms")]
            public List<Room> Rooms = new List<Room>();
        }

        [XmlType("room")]
        public class Room
        {
            [XmlAttribute("runno")]
            public int  RoomNum;
            [XmlElement("roomType")]
            public List<RoomType> RoomTypes = new List<RoomType>();
        }

        public class RoomType
        {
            [XmlAttribute("roomtypecode")]
            public string Code = string.Empty;
            [XmlArray("rateBases")]
            public List<RateBasis> RateBases = new List<RateBasis>();
            [XmlElement("name")]
            public string Name { get; set; } = string.Empty;
        }

        [XmlType("rateBasis")]
        public class RateBasis
        {
            [XmlAttribute("id")]
            public string ID = string.Empty;
            [XmlElement("totalMinimumSelling")]
            public TotalMinimumSelling TotalMinSelling = new TotalMinimumSelling();
            [XmlElement("total")]
            public Total Total = new Total();
            [XmlElement("withinCancellationDeadline")]
            public string WithinCancellationDeadline = string.Empty;
            [XmlElement("rateType")]
            public RateDetails RateType = new RateDetails();
            [XmlElement("minStay")]
            public string MinStay = string.Empty;
            [XmlElement("cancellationRules")]
            public CancellationRules CancellationRules = new CancellationRules();
            [XmlElement("cancellation")]
            public string Cancellation = string.Empty;
        }

        public class CancellationRules
        {
            [XmlElement("rule")]
            public List<Rule> Rule = new List<Rule>();
        }

        public class Rule
        {
            [XmlElement("fromDate")]
            public string FromDate = string.Empty;
            [XmlElement("toDate")]
            public string ToDate = string.Empty;
            [XmlElement("charge")]
            public Charge Charge = new Charge();
            [XmlElement("noShowPolicy")]
            public bool NoShowPolicy;
            [XmlElement("cancelRestricted")]
            public bool CancelRestricted;
        }

        public class Charge
        {
            //[XmlText]
            //public decimal Value;
            [XmlElement("formatted")]
            public string Formatted = string.Empty;
        }

        public class Total
        {
            [XmlText()]
            public string TotalCost = string.Empty;
            [XmlElement("formatted")]
            public string formattedTotal = string.Empty;
        }

        public class TotalMinimumSelling
        {
            [XmlText()]
            public string Total = string.Empty;
            [XmlElement("formatted")]
            public string formattedTotal = string.Empty;
        }

        public class RateDetails
        {
            [XmlAttribute("currencyid")]
            public string CurrencyID = string.Empty;
            [XmlText()]
            public string Type = string.Empty;
        }
    }
}

