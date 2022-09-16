namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    public class SupplierTestSearchResponse
    {
        public Propertyresult[] PropertyResults { get; set; }
    }

    public class Propertyresult
    {
        public string BookingToken { get; set; }
        public int PropertyID { get; set; }
        public Roomtype[] RoomTypes { get; set; }
    }

    public class Roomtype
    {
        public int RoomID { get; set; }
        public string RoomBookingToken { get; set; }
        public string Supplier { get; set; }
        public string MealBasisCode { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeCode { get; set; }
        public string SupplierRoomType { get; set; }
        public string SupplierReference1 { get; set; }
        public string SupplierReference2 { get; set; }
        public string CurrencyCode { get; set; }
        public float TotalCost { get; set; }
        public float Discount { get; set; }
        public string RateCode { get; set; }
        public bool NonRefundable { get; set; }
        public Cancellationterm[] CancellationTerms { get; set; }
        public Adjustment[] Adjustments { get; set; }
        public float CommissionPercentage { get; set; }
        public bool OnRequest { get; set; }
    }

    public class Cancellationterm
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float Amount { get; set; }
    }

    public class Adjustment
    {
        public string AdjustmentType { get; set; }
        public string AdjustmentName { get; set; }
        public float AdjustmentAmount { get; set; }
        public string Description { get; set; }
    }
}