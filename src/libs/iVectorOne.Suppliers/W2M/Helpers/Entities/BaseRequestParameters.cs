namespace ThirdParty.CSSuppliers.Models.W2M
{
    public class BaseRequestParameters
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string Endpoint { get; set; }
        public string SoapPrefix { get; set; }
        public bool CreateLogs { get; set; }
    }
}