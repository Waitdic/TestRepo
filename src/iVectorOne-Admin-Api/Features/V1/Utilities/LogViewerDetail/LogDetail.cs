namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewerDetail
{
    public record LogDetail
    {
        public string RequestLog { get; set; } = string.Empty;

        public string ResponseLog { get; set; } = string.Empty;
    }
}
