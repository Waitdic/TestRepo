using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Config.Responses
{
    public class SupplierResponse
    {
        public bool Success { get; set; }
        
        public List<ConfigurationDTO> Configurations { get; set; } = new List<ConfigurationDTO>();

        public List<string> Warnings { get; set; } = new List<string>();
    }
}