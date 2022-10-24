namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public record ResponseModel : ResponseModelBase
    {
        public List<LogEntry> LogEntries { get; set; } = new List<LogEntry>();

        public bool HasMoreResults { get; set; }
    }
}
