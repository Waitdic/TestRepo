namespace iVectorOne_Admin_Api.Config.Responses
{
    public class AccountSupplierUpdateResponse
    {
        public bool Success { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
}