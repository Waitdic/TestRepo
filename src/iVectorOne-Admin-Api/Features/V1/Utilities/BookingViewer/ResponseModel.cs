namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public record ResponseModel : ResponseModelBase
    {
        public List<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
    }
}
