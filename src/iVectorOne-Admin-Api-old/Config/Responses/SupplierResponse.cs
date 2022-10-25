namespace iVectorOne_Admin_Api.Config.Responses
{
    using iVectorOne_Admin_Api.Config.Models;

    public class SupplierResponse
    {
        public string SupplierName { get; set; } = string.Empty;
        public int SupplierID { get; set; }
        public bool Success { get; set; }

        public List<ConfigurationDTO> Configurations { get; set; } = new();

        public List<string> Warnings { get; set; } = new();
    }
}