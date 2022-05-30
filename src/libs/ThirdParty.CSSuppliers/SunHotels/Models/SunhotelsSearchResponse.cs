namespace ThirdParty.CSSuppliers.SunHotels
{
    using System;
    using System.Xml.Serialization;


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("searchresult")]
    [XmlRoot(Namespace = "http://xml.sunhotels.net/15/", IsNullable = false)]
    public partial class SunhotelsSearchResponse
    {

        private Hotel[] hotelsField;

        public int PropertyRoomBookingID;

        [XmlArrayItem("hotel", IsNullable = false)]
        public Hotel[] hotels
        {
            get
            {
                return hotelsField;
            }
            set
            {
                hotelsField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Hotel
    {

        private int hotelidField;

        private int destination_idField;

        private int resort_idField;

        private int transferField;

        private Roomtype[] roomtypesField;

        private Note[] notesField;

        private decimal? distanceField;

        private object codesField;

        [XmlElement("hotel.id")]
        public int hotelid
        {
            get
            {
                return hotelidField;
            }
            set
            {
                hotelidField = value;
            }
        }

        public int destination_id
        {
            get
            {
                return destination_idField;
            }
            set
            {
                destination_idField = value;
            }
        }

        public int resort_id
        {
            get
            {
                return resort_idField;
            }
            set
            {
                resort_idField = value;
            }
        }

        public int transfer
        {
            get
            {
                return transferField;
            }
            set
            {
                transferField = value;
            }
        }

        [XmlArrayItem("roomtype", IsNullable = false)]
        public Roomtype[] roomtypes
        {
            get
            {
                return roomtypesField;
            }
            set
            {
                roomtypesField = value;
            }
        }

        [XmlArrayItem("note", IsNullable = false)]
        public Note[] notes
        {
            get
            {
                return notesField;
            }
            set
            {
                notesField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public decimal? distance
        {
            get
            {
                return distanceField;
            }
            set
            {
                distanceField = value;
            }
        }

        public object codes
        {
            get
            {
                return codesField;
            }
            set
            {
                codesField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Roomtype
    {

        private int roomtypeIDField;

        private Room[] roomsField;

        [XmlElement("roomtype.ID")]
        public int roomtypeID
        {
            get
            {
                return roomtypeIDField;
            }
            set
            {
                roomtypeIDField = value;
            }
        }

        [XmlArrayItem("room", IsNullable = false)]
        public Room[] rooms
        {
            get
            {
                return roomsField;
            }
            set
            {
                roomsField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Room
    {

        private int idField;

        private int bedsField;

        private int extrabedsField;

        private Meal[] mealsField;

        private Cancellation_policy[] cancellation_policiesField;

        private object notesField;

        private bool isSuperDealField;

        private bool isBestBuyField;

        private PaymentMethods paymentMethodsField;

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

        public int beds
        {
            get
            {
                return bedsField;
            }
            set
            {
                bedsField = value;
            }
        }

        public int extrabeds
        {
            get
            {
                return extrabedsField;
            }
            set
            {
                extrabedsField = value;
            }
        }

        [XmlArrayItem("meal", IsNullable = false)]
        public Meal[] meals
        {
            get
            {
                return mealsField;
            }
            set
            {
                mealsField = value;
            }
        }

        [XmlArrayItem("cancellation_policy", IsNullable = false)]
        public Cancellation_policy[] cancellation_policies
        {
            get
            {
                return cancellation_policiesField;
            }
            set
            {
                cancellation_policiesField = value;
            }
        }

        public object notes
        {
            get
            {
                return notesField;
            }
            set
            {
                notesField = value;
            }
        }

        public bool isSuperDeal
        {
            get
            {
                return isSuperDealField;
            }
            set
            {
                isSuperDealField = value;
            }
        }

        public bool isBestBuy
        {
            get
            {
                return isBestBuyField;
            }
            set
            {
                isBestBuyField = value;
            }
        }

        public PaymentMethods paymentMethods
        {
            get
            {
                return paymentMethodsField;
            }
            set
            {
                paymentMethodsField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Meal
    {

        private int idField;

        private string labelIdField;

        private Prices pricesField;

        private Discount discountField;

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

        [XmlElement(IsNullable = true)]
        public string labelId
        {
            get
            {
                return labelIdField;
            }
            set
            {
                labelIdField = value;
            }
        }

        public Prices prices
        {
            get
            {
                return pricesField;
            }
            set
            {
                pricesField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public Discount discount
        {
            get
            {
                return discountField;
            }
            set
            {
                discountField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Prices
    {

        private Price priceField;

        public Price price
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
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Price
    {

        private string currencyField;

        private int paymentMethodsField;

        private decimal valueField;

        [XmlAttribute()]
        public string currency
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

        [XmlAttribute()]
        public int paymentMethods
        {
            get
            {
                return paymentMethodsField;
            }
            set
            {
                paymentMethodsField = value;
            }
        }

        [XmlText()]
        public decimal Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Discount
    {

        private int typeIdField;

        private Amounts amountsField;

        public int typeId
        {
            get
            {
                return typeIdField;
            }
            set
            {
                typeIdField = value;
            }
        }

        public Amounts amounts
        {
            get
            {
                return amountsField;
            }
            set
            {
                amountsField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Amounts
    {

        private Amount amountField;

        public Amount amount
        {
            get
            {
                return amountField;
            }
            set
            {
                amountField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Amount
    {

        private string currencyField;

        private int paymentMethodsField;

        private decimal valueField;

        [XmlAttribute()]
        public string currency
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

        [XmlAttribute()]
        public int paymentMethods
        {
            get
            {
                return paymentMethodsField;
            }
            set
            {
                paymentMethodsField = value;
            }
        }

        [XmlText()]
        public decimal Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Cancellation_policy
    {

        private string deadlineField;

        private string percentageField;

        [XmlElement(IsNullable = true)]
        public string deadline
        {
            get
            {
                return deadlineField;
            }
            set
            {
                deadlineField = value;
            }
        }

        public string percentage
        {
            get
            {
                return percentageField;
            }
            set
            {
                percentageField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class PaymentMethods
    {

        private PaymentMethod paymentMethodField;

        public PaymentMethod paymentMethod
        {
            get
            {
                return paymentMethodField;
            }
            set
            {
                paymentMethodField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class PaymentMethod
    {

        private int idField;

        [XmlAttribute()]
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
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xml.sunhotels.net/15/")]
    public partial class Note
    {

        private string textField;

        private DateTime start_dateField;

        private DateTime end_dateField;

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

        [XmlAttribute()]
        public DateTime start_date
        {
            get
            {
                return start_dateField;
            }
            set
            {
                start_dateField = value;
            }
        }

        [XmlAttribute()]
        public DateTime end_date
        {
            get
            {
                return end_dateField;
            }
            set
            {
                end_dateField = value;
            }
        }
    }
}