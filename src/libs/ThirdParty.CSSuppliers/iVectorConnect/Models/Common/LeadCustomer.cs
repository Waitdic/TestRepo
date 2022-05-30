namespace ThirdParty.CSSuppliers.iVectorConnect.Models.Common
{
    public class LeadCustomer
    {
        public string CustomerTitle { get; set; } = string.Empty;

        public string CustomerFirstName { get; set; } = string.Empty;

        public string CustomerLastName { get; set; } = string.Empty;

        public string CustomerAddress1 { get; set; } = string.Empty;

        public string CustomerAddress2 { get; set; } = string.Empty;

        public string CustomerTownCity { get; set; } = string.Empty;

        public string CustomerCounty { get; set; } = string.Empty;

        public string CustomerPostcode { get; set; } = string.Empty;

        public int CustomerBookingCountryID { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;
    }
}
