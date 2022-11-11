namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewerDetail
{

    public record ResponseModel : ResponseModelBase
    {
        public List<LogDetail> LogDetails { get; set; } = new List<LogDetail>();

    }
}