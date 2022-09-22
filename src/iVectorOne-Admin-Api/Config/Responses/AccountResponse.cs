namespace iVectorOne_Admin_Api.Config.Responses
{
    using iVectorOne_Admin_Api.Config.Models;

    public class AccountResponse
    {
        public bool Success { get; set; }
        public AccountDTO Account { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}