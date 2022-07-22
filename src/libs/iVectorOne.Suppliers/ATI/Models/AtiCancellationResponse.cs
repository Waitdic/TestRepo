namespace iVectorOne.CSSuppliers.ATI.Models
{
    using iVectorOne.CSSuppliers.ATI.Models.Common;

    public class AtiCancellationResponse : SoapContent
    {
        public string Success { get; set; } = string.Empty;

        public UniqueId UniqueID { get; set; } = new();
    }
}
