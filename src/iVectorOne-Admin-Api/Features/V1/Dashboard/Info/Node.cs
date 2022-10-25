using Amazon.CloudWatchLogs.Model;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record Node
    {
        public int Time { get; set; }

        public int? CurrentTotal { get; set; }

        public int PreviousTotal { get; set; }
        //public string Name { get; set; }
        //public int SearchTotal { get; set; }
        //public int SearchSuccess { get; set; }
        //public int AvgResponse { get; set; }
        //public int PrebookTotal { get; set; }
        //public int PrebookSuccess { get; set; }
        //public int BookTotal { get; set; }
        //public int BookSuccess { get; set; }
        //public int S2B { get; set; }

    }
}