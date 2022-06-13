namespace iVectorOne_Admin_Api.Config.Models
{
    public class SupplierSubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public List<SupplierDTO> SupplierSubscriptions { get; set; } = new List<SupplierDTO>();
    }
}