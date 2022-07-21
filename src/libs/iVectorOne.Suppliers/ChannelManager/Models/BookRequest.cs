namespace ThirdParty.CSSuppliers.ChannelManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.ChannelManager.Models.Common;

    public class BookRequest
    {
        public BookingLogin LoginDetails { get; set; } = new();

        public string BookingReference { get; set; }
        public int PropertyReferenceID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string PromotionalCode { get; set; }
        public List<Room> Rooms { get; set; }
        [XmlArrayItem("HotelRequest")]
        public List<string> HotelRequests { get; set; }
        public LeadGuestDetail LeadGuestDetails { get; set; }
        public PaymentDetail PaymentDetails { get; set; }

        public class Room
        {
            public int Seq { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string ChildAgeCSV { get; set; }
            public string RoomBookingToken { get; set; }
            public string RoomType { get; set; }
            public string MealBasis { get; set; }
            public List<GuestDetail> GuestDetails { get; set; }
        }

        public class LeadGuestDetail
        {
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Telephone { get; set; }
            public string Email { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string TownCity { get; set; }
            public string County { get; set; }
            public string Postcode { get; set; }
            public string Country { get; set; }
            public string ISOCode { get; set; }
        }

        public class GuestDetail
        {
            public string GuestType { get; set; }
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
        }

        public class PaymentDetail
        {
            public decimal Amount { get; set; }
            public string CurrencyCode { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string CardNumber { get; set; }
            public string CVV { get; set; }
            public decimal CreditLimit { get; set; }
            public string CardHolderName { get; set; }
            public string ExpiryMonth { get; set; }
            public string ExpiryYear { get; set; }
            public string Status { get; set; }
        }
    }
}