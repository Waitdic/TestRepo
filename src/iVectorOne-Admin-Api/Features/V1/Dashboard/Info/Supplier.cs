namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record Supplier
    {
        public string Name { get; set; } = string.Empty;

        public string QueryDate { get; set; } = string.Empty;

        public SupplierBookings Bookings { get; set; } = new SupplierBookings();

        public SupplierPrebook Prebook { get; set; } = new SupplierPrebook();

        public SupplierSearches Searches { get; set; } = new SupplierSearches();
        //public string SearchTotal { get; set; } = string.Empty;
        //public string SearchSuccess { get; set; } = string.Empty;
        //public string AvgResponse { get; set; } = string.Empty;
        //public string PrebookTotal { get; set; } = string.Empty;
        //public string PrebookSuccess { get; set; } = string.Empty;
        //public string BookTotal { get; set; } = string.Empty;
        //public string BookSuccess { get; set; } = string.Empty;
        public string S2B { get; set; } = string.Empty;
    }

    public record SupplierBookings
    {
        public string Total { get; set; } = string.Empty;
        public string Successful { get; set; } = string.Empty;
    }

    public record SupplierPrebook
    {
        public string Total { get; set; } = string.Empty;
        public string Successful { get; set; } = string.Empty;
    }

    public record SupplierSearches
    {
        public string Total { get; set; } = string.Empty;
        public string Successful { get; set; } = string.Empty;
        public string AvgResp { get; set; } = string.Empty;
    }

}