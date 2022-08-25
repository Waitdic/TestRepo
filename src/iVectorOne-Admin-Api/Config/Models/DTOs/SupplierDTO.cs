namespace iVectorOne_Admin_Api.Config.Models
{
    public class SupplierDTO
    {
        public int AccountSupplierID { get; set; }
        public int SupplierID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; } = false;
    }
}