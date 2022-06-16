namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    using System.Collections.Generic;

    public class SearchResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();
        public List<Property> Properties { get; set; }

        public class Property
        {
            public int PropertyReferenceID { get; set; }
            public int CurrencyID { get; set; }
            public string ChannelManager { get; set; }
            public List<Room> Rooms { get; set; }
        }

        public class Room
        {
            public int Seq { get; set; }
            public string RoomBookingToken { get; set; }
            public int RateCodeID { get; set; }
            public string RateCode { get; set; }
            public string RateDescription { get; set; }
            public int PropertyRoomTypeID { get; set; }
            public int RoomTypeID { get; set; }
            public string RoomType { get; set; }
            public int RoomViewID { get; set; }
            public string RoomView { get; set; }
            public int MealBasisID { get; set; }
            public decimal NetCost { get; set; }
            public decimal Commission { get; set; }
            public decimal CommissionPercentage { get; set; }
            public bool NonRefundable { get; set; }

            // needed for the token but don't want to actually return
            public int PropertyID { get; set; }
            public bool PackageRate { get; set; }
            public int BrandID { get; set; }
            public decimal TaxAmount { get; set; }
            public string DailyRates { get; set; }
            public List<Adjustment> Adjustments { get; set; }
        }

        public class Adjustment
        {
            public string AdjustmentType { get; set; }
            public int AdjustmentID { get; set; }
            public string AdjustmentName { get; set; }
            public decimal AdjustmentAmount { get; set; }
            public bool PayLocal { get; set; }

            public bool ShouldSerializePropertyID()
            {
                return false;
            }

            public bool ShouldSerializePackageRate()
            {
                return false;
            }

            public bool ShouldSerializeBrandID()
            {
                return false;
            }

            public bool ShouldSerializeTaxAmount()
            {
                return false;
            }
        }
    }
}