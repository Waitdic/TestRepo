namespace iVectorOne.Suppliers.SunHotels
{
    using System;
    using System.Xml.Serialization;


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("searchresult")]
    [XmlRoot(Namespace = "http://xml.sunhotels.net/15/", IsNullable = false)]
    public partial class SunhotelsSearchResponse
    {
        public int PropertyRoomBookingID;

        [XmlArrayItem("hotel", IsNullable = false)]
        public Hotel[] hotels { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Hotel
    {
        [XmlElement("hotel.id")]
        public int hotelid { get; set; }

        public int destination_id { get; set; }

        public int resort_id { get; set; }

        public int transfer { get; set; }

        [XmlArrayItem("roomtype", IsNullable = false)]
        public Roomtype[] roomtypes { get; set; }

        [XmlArrayItem("note", IsNullable = false)]
        public Note[] notes { get; set; }

        [XmlElement(IsNullable = true)]
        public decimal? distance { get; set; }

        public object codes { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Roomtype
    {
        [XmlElement("roomtype.ID")]
        public int roomtypeID { get; set; }

        [XmlArrayItem("room", IsNullable = false)]
        public Room[] rooms { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Room
    {
        public int id { get; set; }

        public int beds { get; set; }

        public int extrabeds { get; set; }

        [XmlArrayItem("meal", IsNullable = false)]
        public Meal[] meals { get; set; }

        [XmlArrayItem("cancellation_policy", IsNullable = false)]
        public Cancellation_policy[] cancellation_policies { get; set; }

        public object notes { get; set; }

        public bool isSuperDeal { get; set; }

        public bool isBestBuy { get; set; }

        public PaymentMethods paymentMethods { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Meal
    {
        public int id { get; set; }

        [XmlElement(IsNullable = true)]
        public string labelId { get; set; }

        public Prices prices { get; set; }

        [XmlElement(IsNullable = true)]
        public Discount discount { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Prices
    {
        public Price price { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Price
    {
        [XmlAttribute()]
        public string currency { get; set; }

        [XmlAttribute()]
        public int paymentMethods { get; set; }

        [XmlText()]
        public decimal Value { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Discount
    {
        public int typeId { get; set; }

        public Amounts amounts { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Amounts
    {
        public Amount amount { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Amount
    {
        [XmlAttribute()]
        public string currency { get; set; }

        [XmlAttribute()]
        public int paymentMethods { get; set; }

        [XmlText()]
        public decimal Value { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Cancellation_policy
    {
        [XmlElement(IsNullable = true)]
        public string deadline { get; set; }

        public string percentage { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class PaymentMethods
    {
        public PaymentMethod paymentMethod { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class PaymentMethod
    {
        [XmlAttribute()]
        public int id { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Note
    {
        public string text { get; set; }

        [XmlAttribute()]
        public DateTime start_date { get; set; }

        [XmlAttribute()]
        public DateTime end_date { get; set; }
    }
}