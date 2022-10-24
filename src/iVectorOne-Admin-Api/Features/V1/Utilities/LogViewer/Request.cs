using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public record Request : IRequest<ResponseBase>
    {
        public int AccountID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Environment { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

    }
}
