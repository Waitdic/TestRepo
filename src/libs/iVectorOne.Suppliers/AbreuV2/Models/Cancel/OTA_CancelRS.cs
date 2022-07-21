namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    public class OTA_CancelRS
    {
        public CancelInfoRS CancelInfoRS { get; set; }
    }

    public class CancelInfoRS
    {
        public CancellationCosts CancellationCosts { get; set; } = new();
    }
}
