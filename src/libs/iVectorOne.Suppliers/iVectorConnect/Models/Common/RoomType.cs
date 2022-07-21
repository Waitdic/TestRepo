namespace iVectorOne.CSSuppliers.iVectorConnect.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomType
    {
        public int Seq { get; set; }
        public string RoomBookingToken { get; set; } = string.Empty;
        public int MealBasisID { get; set; }
        public int RoomTypeID { get; set; }

        [XmlElement("RoomType")]
        public string RoomTypeProperty { get; set; } = string.Empty;
        public string RoomView { get; set; } = string.Empty;
        public int AvailableRooms { get; set; }
        public int DiscountID { get; set; }
        public decimal Discount { get; set; }
        public decimal Saving { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal PayLocalTotal { get; set; }
        public bool NonRefundable { get; set; }
        public string SpecialOffer { get; set; } = string.Empty;
        public SupplierDetails SupplierDetails { get; set; } = new();
        public List<Adjustment> Adjustments { get; set; } = new();
        public List<Cancellation> SupplierCancellations { get; set; } = new();
    }
}