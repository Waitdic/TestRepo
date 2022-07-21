namespace iVectorOne.Suppliers.ATI.Models
{
    using iVectorOne.Suppliers.ATI.Models.Common;

    public class AtiCancellationResponse : SoapContent
    {
        public string Success { get; set; } = string.Empty;

        public UniqueId UniqueID { get; set; } = new();
    }
}
