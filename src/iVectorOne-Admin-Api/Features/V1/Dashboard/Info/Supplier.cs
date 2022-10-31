namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record Supplier
    {
        public string Name { get; set; } = string.Empty;

        public string QueryDate { get; set; } = string.Empty;

        public string SearchTotal { get; set; } = string.Empty;
        public string SearchSuccess { get; set; } = string.Empty;
        public string AvgResponse { get; set; } = string.Empty;
        public string PrebookTotal { get; set; } = string.Empty;
        public string PrebookSuccess { get; set; } = string.Empty;
        public string BookTotal { get; set; } = string.Empty;
        public string BookSuccess { get; set; } = string.Empty;
        public string S2B { get; set; } = string.Empty;




    }
}