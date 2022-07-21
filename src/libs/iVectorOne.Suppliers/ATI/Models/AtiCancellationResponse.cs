namespace ThirdParty.CSSuppliers.ATI.Models
{
    using ThirdParty.CSSuppliers.ATI.Models.Common;

    public class AtiCancellationResponse : SoapContent
    {
        public string Success { get; set; } = string.Empty;

        public UniqueId UniqueID { get; set; } = new();
    }
}
