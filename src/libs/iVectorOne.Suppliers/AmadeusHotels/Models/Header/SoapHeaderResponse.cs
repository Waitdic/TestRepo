namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Header
{
    public class SoapHeaderResponse
    {
        public string To { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string MessageID { get; set; } = string.Empty;

        public Session Session { get; set; } = new();
    }
}
