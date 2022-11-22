using iVectorOne_Admin_Api.Features;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record ResponseModel : ResponseModelBase
    {
        public List<Node> BookingsByHour { get; set; } = new List<Node>();

        public List<Node> SearchesByHour { get; set; } = new List<Node>();

        public List<Summary> Summary { get; set; } = new List<Summary>();

        public List<Supplier> Supplier { get; set; } = new List<Supplier>();
    }

    #region DTO

    public record Node
    {
        public int Time { get; set; }

        public int? CurrentTotal { get; set; }

        public int PreviousTotal { get; set; }
    }

    public record Summary
    {
        public string Name { get; set; } = string.Empty;

        public Bookings Bookings { get; set; } = new Bookings();

        public Prebook Prebook { get; set; } = new Prebook();

        public Searches Searches { get; set; } = new Searches();

        public string S2B { get; set; } = string.Empty;
    }

    public record Bookings
    {
        public string Total { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public record Prebook
    {
        public string Total { get; set; } = string.Empty;
        public string Successful { get; set; } = string.Empty;
    }

    public record Searches
    {
        public string Total { get; set; } = string.Empty;
        public string Successful { get; set; } = string.Empty;
        public string Avg_Resp { get; set; } = string.Empty;
    }

    public record Supplier
    {
        public string Name { get; set; } = string.Empty;

        public string QueryDate { get; set; } = string.Empty;

        public SupplierBookings Bookings { get; set; } = new SupplierBookings();

        public SupplierPrebook Prebook { get; set; } = new SupplierPrebook();

        public SupplierSearches Searches { get; set; } = new SupplierSearches();

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
        public string Avg_Resp { get; set; } = string.Empty;
    }
    #endregion
}