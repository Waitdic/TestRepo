namespace ThirdParty.CSSuppliers.YouTravel
{
    using System;
    using System.Xml.Serialization;
    using Intuitive.Helpers.Extensions;

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("HtSearchRq")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class YouTravelSearchResponse
    {

        private string successField;

        private Search_Criteria search_CriteriaField;

        private Session sessionField;

        public string Success
        {
            get
            {
                return successField;
            }
            set
            {
                successField = value;
            }
        }

        public Search_Criteria Search_Criteria
        {
            get
            {
                return search_CriteriaField;
            }
            set
            {
                search_CriteriaField = value;
            }
        }

        public Session session
        {
            get
            {
                return sessionField;
            }
            set
            {
                sessionField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Search_Criteria
    {

        private string companyField;

        private string langIDField;

        private string dstnField;

        private object rsrtField;

        private byte roomsField;

        private byte nofnField;

        private string checkin_DateField;

        private byte youtravel_RatingField;

        private string board_TypeField;

        private byte adlts_1Field;

        private byte child_1Field;

        private byte infant_1Field;

        private byte adlts_2Field;

        private byte child_2Field;

        private byte infant_2Field;

        private byte adlts_3Field;

        private byte child_3Field;

        private byte infant_3Field;

        public string Company
        {
            get
            {
                return companyField;
            }
            set
            {
                companyField = value;
            }
        }

        public string LangID
        {
            get
            {
                return langIDField;
            }
            set
            {
                langIDField = value;
            }
        }

        public string Dstn
        {
            get
            {
                return dstnField;
            }
            set
            {
                dstnField = value;
            }
        }

        public object Rsrt
        {
            get
            {
                return rsrtField;
            }
            set
            {
                rsrtField = value;
            }
        }

        public byte Rooms
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

        public byte Nofn
        {
            get
            {
                return nofnField;
            }
            set
            {
                nofnField = value;
            }
        }

        public string Checkin_Date
        {
            get
            {
                return checkin_DateField;
            }
            set
            {
                checkin_DateField = value;
            }
        }

        public byte Youtravel_Rating
        {
            get
            {
                return youtravel_RatingField;
            }
            set
            {
                youtravel_RatingField = value;
            }
        }

        public string Board_Type
        {
            get
            {
                return board_TypeField;
            }
            set
            {
                board_TypeField = value;
            }
        }

        public byte Adlts_1
        {
            get
            {
                return adlts_1Field;
            }
            set
            {
                adlts_1Field = value;
            }
        }

        public byte Child_1
        {
            get
            {
                return child_1Field;
            }
            set
            {
                child_1Field = value;
            }
        }

        public byte Infant_1
        {
            get
            {
                return infant_1Field;
            }
            set
            {
                infant_1Field = value;
            }
        }

        public byte Adlts_2
        {
            get
            {
                return adlts_2Field;
            }
            set
            {
                adlts_2Field = value;
            }
        }

        public byte Child_2
        {
            get
            {
                return child_2Field;
            }
            set
            {
                child_2Field = value;
            }
        }

        public byte Infant_2
        {
            get
            {
                return infant_2Field;
            }
            set
            {
                infant_2Field = value;
            }
        }

        public byte Adlts_3
        {
            get
            {
                return adlts_3Field;
            }
            set
            {
                adlts_3Field = value;
            }
        }

        public byte Child_3
        {
            get
            {
                return child_3Field;
            }
            set
            {
                child_3Field = value;
            }
        }

        public byte Infant_3
        {
            get
            {
                return infant_3Field;
            }
            set
            {
                infant_3Field = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Session
    {

        private string currencyField;

        private Hotel[] hotelField;

        private string idField;

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

        [XmlElement("Hotel")]
        public Hotel[] Hotel
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

        [XmlAttribute(DataType = "integer")]
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
    [XmlType(AnonymousType = true)]
    public partial class Hotel
    {

        private string hotel_NameField;

        private string youtravel_RatingField;

        private string official_RatingField;

        private string board_TypeField;

        private string child_AgeField;

        private string countryField;

        private string destinationField;

        private string resortField;

        private string imageField;

        private string hotel_DescField;

        private Room_1 room_1Field;

        private Room_2 room_2Field;

        private Room_3 room_3Field;

        private string idField;

        public string Hotel_Name
        {
            get
            {
                return hotel_NameField;
            }
            set
            {
                hotel_NameField = value;
            }
        }

        public string Youtravel_Rating
        {
            get
            {
                return youtravel_RatingField;
            }
            set
            {
                youtravel_RatingField = value;
            }
        }

        public string Official_Rating
        {
            get
            {
                return official_RatingField;
            }
            set
            {
                official_RatingField = value;
            }
        }

        public string Board_Type
        {
            get
            {
                return board_TypeField;
            }
            set
            {
                board_TypeField = value;
            }
        }

        public string Child_Age
        {
            get
            {
                return child_AgeField;
            }
            set
            {
                child_AgeField = value;
            }
        }

        public string Country
        {
            get
            {
                return countryField;
            }
            set
            {
                countryField = value;
            }
        }

        public string Destination
        {
            get
            {
                return destinationField;
            }
            set
            {
                destinationField = value;
            }
        }

        public string Resort
        {
            get
            {
                return resortField;
            }
            set
            {
                resortField = value;
            }
        }

        public string Image
        {
            get
            {
                return imageField;
            }
            set
            {
                imageField = value;
            }
        }

        public string Hotel_Desc
        {
            get
            {
                return hotel_DescField;
            }
            set
            {
                hotel_DescField = value;
            }
        }

        public Room_1 Room_1
        {
            get
            {
                return room_1Field;
            }
            set
            {
                room_1Field = value;
            }
        }

        public Room_2 Room_2
        {
            get
            {
                return room_2Field;
            }
            set
            {
                room_2Field = value;
            }
        }

        public Room_3 Room_3
        {
            get
            {
                return room_3Field;
            }
            set
            {
                room_3Field = value;
            }
        }

        [XmlAttribute()]
        public string ID
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
    [XmlType(AnonymousType = true)]
    public partial class Room_1
    {

        private Passengers passengersField;

        private Room[] roomField;

        public Passengers Passengers
        {
            get
            {
                return passengersField;
            }
            set
            {
                passengersField = value;
            }
        }

        [XmlElement("Room")]
        public Room[] Room
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
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Passengers
    {

        private byte adultsField;

        private byte childrenField;

        private byte infantsField;

        [XmlAttribute()]
        public byte Adults
        {
            get
            {
                return adultsField;
            }
            set
            {
                adultsField = value;
            }
        }

        [XmlAttribute()]
        public byte Children
        {
            get
            {
                return childrenField;
            }
            set
            {
                childrenField = value;
            }
        }

        [XmlAttribute()]
        public byte Infants
        {
            get
            {
                return infantsField;
            }
            set
            {
                infantsField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room
    {

        private CanxPolicy canxPolicyField;

        private string typeField;

        private string boardField;

        private string aI_TypeField;

        private Rates ratesField;

        private Offers offersField;

        private string idField;

        private string refundableField;

        public CanxPolicy CanxPolicy
        {
            get
            {
                return canxPolicyField;
            }
            set
            {
                canxPolicyField = value;
            }
        }

        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }

        public string Board
        {
            get
            {
                return boardField;
            }
            set
            {
                boardField = value;
            }
        }

        public string AI_Type
        {
            get
            {
                return aI_TypeField;
            }
            set
            {
                aI_TypeField = value;
            }
        }

        public Rates Rates
        {
            get
            {
                return ratesField;
            }
            set
            {
                ratesField = value;
            }
        }

        public Offers Offers
        {
            get
            {
                return offersField;
            }
            set
            {
                offersField = value;
            }
        }

        [XmlAttribute()]
        public string Id
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

        [XmlAttribute()]
        public string Refundable
        {
            get
            {
                return refundableField;
            }
            set
            {
                refundableField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class CanxPolicy
    {

        private string tokenField;

        [XmlAttribute()]
        public string token
        {
            get
            {
                return tokenField;
            }
            set
            {
                tokenField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Rates
    {

        private decimal original_RateField;

        private decimal final_RateField;

        [XmlAttribute()]
        public decimal Original_Rate
        {
            get
            {
                return original_RateField;
            }
            set
            {
                original_RateField = value;
            }
        }

        [XmlAttribute()]
        public decimal Final_Rate
        {
            get
            {
                return final_RateField;
            }
            set
            {
                final_RateField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Offers
    {

        private byte lastminute_OfferField;

        private byte early_Booking_DiscountField;

        private byte free_StayField;

        private byte free_TransferField;

        private byte gala_MealsField;

        [XmlAttribute()]
        public byte Lastminute_Offer
        {
            get
            {
                return lastminute_OfferField;
            }
            set
            {
                lastminute_OfferField = value;
            }
        }

        [XmlAttribute()]
        public byte Early_Booking_Discount
        {
            get
            {
                return early_Booking_DiscountField;
            }
            set
            {
                early_Booking_DiscountField = value;
            }
        }

        [XmlAttribute()]
        public byte Free_Stay
        {
            get
            {
                return free_StayField;
            }
            set
            {
                free_StayField = value;
            }
        }

        [XmlAttribute()]
        public byte Free_Transfer
        {
            get
            {
                return free_TransferField;
            }
            set
            {
                free_TransferField = value;
            }
        }

        [XmlAttribute()]
        public byte Gala_Meals
        {
            get
            {
                return gala_MealsField;
            }
            set
            {
                gala_MealsField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room_2
    {

        private Passengers passengersField;

        private Room[] roomField;

        public Passengers Passengers
        {
            get
            {
                return passengersField;
            }
            set
            {
                passengersField = value;
            }
        }

        [XmlElement("Room")]
        public Room[] Room
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
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Room_3
    {

        private Passengers passengersField;

        private Room[] roomField;

        public Passengers Passengers
        {
            get
            {
                return passengersField;
            }
            set
            {
                passengersField = value;
            }
        }

        [XmlElement("Room")]
        public Room[] Room
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
    }

}