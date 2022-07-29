namespace iVectorOne.Suppliers.Travelgate.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable()]
    [XmlRoot("Envelope")]
    public class TravelgateResponseEnvelope
    {
        [XmlElement("Body")]
        public TravelgateSearchEncodedResponse Body { get; set; }
    }

    [Serializable()]
    [XmlType("Body")]
    public class TravelgateSearchEncodedResponse
    {
        [XmlElement("AvailResponse")]
        public ResponseDetails Response { get; set; }

        public class ResponseDetails
        {
            [XmlElement("AvailResult")]
            public ProviderRSs Result { get; set; }
        }

        public class ProviderRSs
        {
            [XmlElement("providerRSs")]
            public List<ProviderResultsDetails> ProviderResults { get; set; } = new List<ProviderResultsDetails>();
        }

        public class ProviderResultsDetails
        {
            [XmlElement("ProviderRS")]
            public ResultDetails Results { get; set; }
        }

        public class ResultDetails
        {
            [XmlElement("rs")]
            public string Result { get; set; }
        }

    }

    [Serializable()]
    [XmlRoot("AvailRS")]
    public class TravelgateSearchResponse
    {
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
        [XmlArray("DailyRatePlans")]
        [XmlArrayItem("DailyRatePlan")]
        public List<RatePlan> DailyRatePlans { get; set; } = new List<RatePlan>();

        public class Hotel
        {
            [XmlAttribute("code")]
            public string TPKey { get; set; }
            [XmlAttribute("name")]
            public string Name { get; set; }
            [XmlArray("MealPlans")]
            [XmlArrayItem("MealPlan")]
            public List<Meal> Meals { get; set; } = new List<Meal>();
        }

        public class Meal
        {
            [XmlAttribute("code")]
            public string MealBaisCode { get; set; }
            [XmlArray("Options")]
            [XmlArrayItem("Option")]
            public List<OptionDetails> Options { get; set; } = new List<OptionDetails>();
        }

        public class OptionDetails
        {
            [XmlAttribute("type")]
            public string Type { get; set; }
            [XmlAttribute("paymentType")]
            public string PaymentType { get; set; }
            [XmlAttribute("status")]
            public string Status { get; set; }
            public List<Room> Rooms { get; set; } = new List<Room>();
            [XmlElement("Price")]
            public PriceDetails Price { get; set; }
            public List<Parameter> Parameters { get; set; } = new List<Parameter>();
            public List<RateRule> RateRules { get; set; } = new List<RateRule>();
        }

        public class Room
        {
            [XmlAttribute("id")]
            public string ID { get; set; }
            [XmlAttribute("code")]
            public string RoomTypeCode { get; set; }
            [XmlAttribute("description")]
            public string RoomType { get; set; }
            [XmlAttribute("roomCandidateRefId")]
            public string PropertyRoomBookingID { get; set; }
            [XmlAttribute("nonRefundable")]
            public string NonRefunfable { get; set; } = string.Empty;
        }

        public class PriceDetails
        {
            [XmlAttribute("currency")]
            public string Currency { get; set; }
            [XmlAttribute("amount")]
            public string Amount { get; set; }
            [XmlAttribute("binding")]
            public string Binding { get; set; }
            [XmlAttribute("commission")]
            public string Commission { get; set; }
        }

        public class Parameter
        {
            [XmlAttribute("key")]
            public string Key { get; set; }
            [XmlAttribute("value")]
            public string Value { get; set; }
        }

        public class RateRule
        {
            [XmlElement("Rules")]
            public List<Rule> Rules { get; set; } = new List<Rule>();
        }

        public class Rule
        {
            [XmlAttribute("type")]
            public string RateType { get; set; }
        }

        public class RatePlan
        {
            [XmlAttribute("code")]
            public string TPRateCode { get; set; } = string.Empty;
        }
    }
}