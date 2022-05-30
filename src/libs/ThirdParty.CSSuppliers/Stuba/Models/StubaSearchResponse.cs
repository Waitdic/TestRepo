namespace ThirdParty.CSSuppliers.Stuba
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType("AvailabilitySearchResult")]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class StubaSearchResponse
    {

        private string currencyField;

        private object warningField;

        private bool testModeField;

        private List<HotelAvailability> hotelAvailabilityField;


        public string Currency
        {
            get
            {
                return currencyField;
            }
            set
            {
                currencyField = value;
            }
        }


        public object Warning
        {
            get
            {
                return warningField;
            }
            set
            {
                warningField = value;
            }
        }


        public bool TestMode
        {
            get
            {
                return testModeField;
            }
            set
            {
                testModeField = value;
            }
        }


        [System.Xml.Serialization.XmlElement("HotelAvailability")]
        public List<HotelAvailability> HotelAvailability
        {
            get
            {
                return hotelAvailabilityField;
            }
            set
            {
                hotelAvailabilityField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class HotelAvailability
    {

        private Hotel hotelField;

        private List<Result> resultField;

        private string hotelQuoteIdField;


        public Hotel Hotel
        {
            get
            {
                return hotelField;
            }
            set
            {
                hotelField = value;
            }
        }


        [System.Xml.Serialization.XmlElement("Result")]
        public List<Result> Result
        {
            get
            {
                return resultField;
            }
            set
            {
                resultField = value;
            }
        }


        [System.Xml.Serialization.XmlAttribute()]
        public string hotelQuoteId
        {
            get
            {
                return hotelQuoteIdField;
            }
            set
            {
                hotelQuoteIdField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class Hotel
    {

        private int idField;

        private string nameField;


        [System.Xml.Serialization.XmlAttribute()]
        public int id
        {
            get
            {
                return idField;
            }
            set
            {
                idField = value;
            }
        }


        [System.Xml.Serialization.XmlAttribute()]
        public string name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class Result
    {

        private List<Room> roomField;

        private string idField;


        [System.Xml.Serialization.XmlElement("Room")]
        public List<Room> Room
        {
            get
            {
                return roomField;
            }
            set
            {
                roomField = value;
            }
        }


        [System.Xml.Serialization.XmlAttribute()]
        public string id
        {
            get
            {
                return idField;
            }
            set
            {
                idField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class Room
    {

        private RoomType roomTypeField;

        private MealType mealTypeField;

        private Price priceField;

        private object messagesField;

        private string cancellationPolicyStatusField;


        public RoomType RoomType
        {
            get
            {
                return roomTypeField;
            }
            set
            {
                roomTypeField = value;
            }
        }


        public MealType MealType
        {
            get
            {
                return mealTypeField;
            }
            set
            {
                mealTypeField = value;
            }
        }


        public Price Price
        {
            get
            {
                return priceField;
            }
            set
            {
                priceField = value;
            }
        }


        public object Messages
        {
            get
            {
                return messagesField;
            }
            set
            {
                messagesField = value;
            }
        }


        public string CancellationPolicyStatus
        {
            get
            {
                return cancellationPolicyStatusField;
            }
            set
            {
                cancellationPolicyStatusField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class RoomType
    {

        private int codeField;

        private string textField;


        [System.Xml.Serialization.XmlAttribute()]
        public int code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }


        [System.Xml.Serialization.XmlAttribute()]
        public string text
        {
            get
            {
                return textField;
            }
            set
            {
                textField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class MealType
    {

        private int codeField;

        private string textField;


        [System.Xml.Serialization.XmlAttribute()]
        public int code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }


        [System.Xml.Serialization.XmlAttribute()]
        public string text
        {
            get
            {
                return textField;
            }
            set
            {
                textField = value;
            }
        }
    }


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class Price
    {

        private decimal amtField;


        [System.Xml.Serialization.XmlAttribute()]
        public decimal amt
        {
            get
            {
                return amtField;
            }
            set
            {
                amtField = value;
            }
        }
    }
}