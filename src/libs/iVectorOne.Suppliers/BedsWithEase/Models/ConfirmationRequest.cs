namespace iVectorOne.CSSuppliers.BedsWithEase.Models
{
    using iVectorOne.CSSuppliers.BedsWithEase.Models.Common;

    public class ConfirmationRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;

        public BookCodesToConfirm BookCodesToConfirm { get; set; } = new();

        public AgencyAddress AgencyAddress { get; set; } = new();

        public AgencyContactDetails AgencyContactDetails { get; set; } = new ();

        public string AgencyReference { get; set; } = string.Empty;
    }
}
