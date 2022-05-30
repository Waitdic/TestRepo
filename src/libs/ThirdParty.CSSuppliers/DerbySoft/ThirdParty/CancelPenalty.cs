namespace ThirdPartyInterfaces.DerbySoft.ThirdParty
{
    using Newtonsoft.Json;

    public class CancelPenalty
    {
        [JsonProperty("noShow")]
        public bool NoShow { get; set; }

        [JsonProperty("cancellable")]
        public bool Cancellable { get; set; }

        [JsonProperty("cancelDeadline")]
        public CancelDeadline CancelDeadline { get; set; }

        [JsonProperty("penaltyCharge")]
        public PenaltyCharge PenaltyCharge { get; set; }
    }
}