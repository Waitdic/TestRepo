namespace ThirdParty.CSSuppliers.Stuba
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("AvailabilitySearchResult")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class StubaSearchResponse
    {
        public string Currency { get; set; }

        public string Warning { get; set; }

        public bool TestMode { get; set; }

        [XmlElement("HotelAvailability")]
        public List<HotelAvailability> HotelAvailability { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class HotelAvailability
    {
        public Hotel Hotel { get; set; }

        [XmlElement("Result")]
        public List<Result> Result { get; set; }

        [XmlAttribute()]
        public string hotelQuoteId { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Hotel
    {
        [XmlAttribute()]
        public int id { get; set; }

        [XmlAttribute()]
        public string name { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Result
    {
        [XmlElement("Room")]
        public List<Room> Room { get; set; }

        [XmlAttribute()]
        public string id { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room
    {
        public RoomType RoomType { get; set; }

        public MealType MealType { get; set; }

        public Price Price { get; set; }

        public string Messages { get; set; }

        public string CancellationPolicyStatus { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RoomType
    {
        [XmlAttribute()]
        public int code { get; set; }

        [XmlAttribute()]
        public string text { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class MealType
    {
        [XmlAttribute()]
        public int code { get; set; }

        [XmlAttribute()]
        public string text { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Price
    {
        [XmlAttribute()]
        public decimal amt { get; set; }
    }
}