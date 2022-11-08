namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
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
}