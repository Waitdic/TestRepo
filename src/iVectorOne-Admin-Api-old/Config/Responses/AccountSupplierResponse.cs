namespace iVectorOne_Admin_Api.Config.Responses
{
    using iVectorOne_Admin_Api.Config.Models;

    public class AccountSupplierResponse
    {
        public bool Success { get; set; }
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<SupplierDTO> AccountSuppliers { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}