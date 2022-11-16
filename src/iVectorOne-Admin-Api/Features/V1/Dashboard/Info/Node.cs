using Amazon.CloudWatchLogs.Model;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record Node
    {
        public int Time { get; set; }

        public int? CurrentTotal { get; set; }

        public int PreviousTotal { get; set; }
    }
}