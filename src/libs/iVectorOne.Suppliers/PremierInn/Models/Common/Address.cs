namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    public class Address
    {
        public string AddressLine1 { get; set; } = string.Empty;

        public string AddressLine2 { get; set; } = string.Empty;

        public string AddressLine3 { get; set; } = string.Empty;

        public string AddressLine4 { get; set; } = string.Empty;

        public string AddressLine5 { get; set; } = string.Empty;
        public bool ShouldSerializeAddressLine5() => !string.IsNullOrEmpty(AddressLine5);

        public string PostCode { get; set; } = string.Empty;

        public string FaxNumber { get; set; } = string.Empty;
        public bool ShouldSerializeFaxNumber() => !string.IsNullOrEmpty(FaxNumber);

        public string TelephoneNumber { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;
    }
}

