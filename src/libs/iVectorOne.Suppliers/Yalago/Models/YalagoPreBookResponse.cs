namespace ThirdParty.CSSuppliers.Models.Yalago
{
using System;
#pragma warning disable CS8618
#pragma warning disable CS0649

    class YalagoPreBookResponse
    {
        public InfoItem[] InfoItems;
        public Establishment establishment;
        public class Establishment
        {
            public int EstablishmentId { get; set; }
            public Room[] Rooms { get; set; }
            public EstablishmentInfo establishmentInfo { get; set; }
            public string[] Messages { get; set; }
        }

        public class Room
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public int QuantityAvailable { get; set; }
            public Board[] Boards { get; set; }
        }
        public class Board
        {
            public int Type { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public NetCost netCost { get; set; }
            public bool NonRefundable { get; set; }
            public bool IsPayAtHotel { get; set; }
            public bool IsBindingPrice { get; set; }
            public bool IsOnRequest { get; set; }
            public int RequestedRoomIndex { get; set; }
            public CancellationPolicy cancellationPolicy { get; set; }
            public string BookingConditions { get; set; }
            public string SpecialOfferText { get; set; }
            public bool IsPackagePrice { get; set; }
            public YalagoLocalCharge[] LocalCharges { get; set; }
            public BoardBasis boardBasis { get; set; }

        }
        public class CancellationPolicy
        {
            public CancellationCharge[] CancellationCharges { get; set; }
        }
        public class CancellationCharge
        {
            public Charge charge { get; set; }
            public DateTime ExpiryDate { get; set; }
            public DateTime ExpiryDateUTC { get; set; }
        }
        public class Charge
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public TaxBreakDown taxBreakDown { get; set; }
        }
        public class TaxBreakDown
        {
            public decimal Amount { get; set; }
            public decimal Vat { get; set; }
        }
        public class NetCost
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }
        public class EstablishmentInfo
        {
            public string EstablishmentName { get; set; }
            public int LocationId { get; set; }
            public string LocationName { get; set; }
            public int Rating { get; set; }
            public int RatingTypeId { get; set; }
            public string AccomadationType { get; set; }
            public string CountryCode { get; set; }

        }

        public class BoardBasis
        {

        }
        public class InfoItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }
    }
}