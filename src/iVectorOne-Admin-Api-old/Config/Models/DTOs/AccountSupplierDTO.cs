namespace iVectorOne_Admin_Api.Config.Models
{
    public class AccountSupplierDTO
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public List<SupplierDTO> Suppliers { get; set; } = new();
    }
}