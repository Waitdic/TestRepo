namespace iVectorOne.Suppliers.YouTravel
{
    using System;
    using System.Xml.Serialization;

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("HtSearchRq")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class YouTravelSearchResponse
    {
        public string Success { get; set; }

        public Search_Criteria Search_Criteria { get; set; }

        public Session session { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Search_Criteria
    {
        public string Company { get; set; }

        public string LangID { get; set; }

        public string Dstn { get; set; }

        public string Rsrt { get; set; }

        public int Rooms { get; set; }

        public int Nofn { get; set; }

        public string Checkin_Date { get; set; }

        public string Youtravel_Rating { get; set; }

        public string Board_Type { get; set; }

        public int Adlts_1 { get; set; }

        public int Child_1 { get; set; }

        public int Infant_1 { get; set; }

        public int Adlts_2 { get; set; }

        public int Child_2 { get; set; }

        public int Infant_2 { get; set; }

        public int Adlts_3 { get; set; }

        public int Child_3 { get; set; }

        public int Infant_3 { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Session
    {
        public string Currency { get; set; }

        [XmlElement("Hotel")]
        public Hotel[] Hotel { get; set; }

        [XmlAttribute(DataType = "integer")]
        public string id { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Hotel
    {
        public string Hotel_Name { get; set; }

        public string Youtravel_Rating { get; set; }

        public string Official_Rating { get; set; }

        public string Board_Type { get; set; }

        public string Child_Age { get; set; }

        public string Country { get; set; }

        public string Destination { get; set; }

        public string Resort { get; set; }

        public string Image { get; set; }

        public string Hotel_Desc { get; set; }

        public Room_1 Room_1 { get; set; }

        public Room_2 Room_2 { get; set; }

        public Room_3 Room_3 { get; set; }

        [XmlAttribute()]
        public string ID { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room_1
    {
        public Passengers Passengers { get; set; }

        [XmlElement("Room")]
        public Room[] Room { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Passengers
    {
        [XmlAttribute()]
        public int Adults { get; set; }

        [XmlAttribute()]
        public int Children { get; set; }

        [XmlAttribute()]
        public int Infants { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room
    {
        public CanxPolicy CanxPolicy { get; set; }

        public string Type { get; set; }

        public string Board { get; set; }

        public string AI_Type { get; set; }

        public Rates Rates { get; set; }

        public Offers Offers { get; set; }

        [XmlAttribute()]
        public string Id { get; set; }

        [XmlAttribute()]
        public string Refundable { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class CanxPolicy
    {
        [XmlAttribute()]
        public string token { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Rates
    {
        [XmlAttribute()]
        public decimal Original_Rate { get; set; }

        [XmlAttribute()]
        public decimal Final_Rate { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Offers
    {
        [XmlAttribute()]
        public bool Lastminute_Offer { get; set; }

        [XmlAttribute()]
        public bool Early_Booking_Discount { get; set; }

        [XmlAttribute()]
        public bool Free_Stay { get; set; }

        [XmlAttribute()]
        public bool Free_Transfer { get; set; }

        [XmlAttribute()]
        public bool Gala_Meals { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room_2
    {
        public Passengers Passengers { get; set; }

        [XmlElement("Room")]
        public Room[] Room { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room_3
    {
        public Passengers Passengers { get; set; }

        [XmlElement("Room")]
        public Room[] Room { get; set; }
    }
}