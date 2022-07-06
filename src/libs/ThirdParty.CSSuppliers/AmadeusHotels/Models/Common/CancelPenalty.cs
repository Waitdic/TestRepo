namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    public class CancelPenalty
    {
        public Deadline Deadline { get; set; } = new();

        public AmountPercent? AmountPercent { get; set; }
        public bool ShouldSerializeAmountPercent => AmountPercent != null;

        public PenaltyDescription PenaltyDescription { get; set; } = new();
    }
}
