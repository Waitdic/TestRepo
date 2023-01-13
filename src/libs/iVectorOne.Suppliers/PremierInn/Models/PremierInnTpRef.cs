namespace iVectorOne.Suppliers.PremierInn.Models
{
    using System;
    using Intuitive.Helpers.Security;
    using Newtonsoft.Json;
    using iVectorOne.Suppliers.PremierInn.Models.Common;

    public class PremierInnTpRef
    {
        public PremierInnTpRef(string sessionId, string rateCode, CancellationPolicy cancellation, string[] statusWarningFlag)
        {
            SessionId = sessionId;
            RateCode = rateCode;
            StatusWarningFlag = statusWarningFlag;
            CancellationPolicy = new Cancellation
            {
                Category = cancellation.Category,
                Days = cancellation.Days,
                Text = cancellation.Text
            };
        }

        public PremierInnTpRef()
        {

        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SessionId { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RateCode { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Cancellation? CancellationPolicy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] StatusWarningFlag { get; set; } = Array.Empty<string>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LeadGuestLastName { get; set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ArrivalDate { get; set; } = string.Empty;

        public string Encrypt(ISecretKeeper secretKeeper)
        {
            return secretKeeper.Encrypt(JsonConvert.SerializeObject(this));
        }

        public static PremierInnTpRef Decrypt(ISecretKeeper secretKeeper, string encTpRef)
        {
            return JsonConvert.DeserializeObject<PremierInnTpRef>(secretKeeper.Decrypt(encTpRef));
        }
    }

    public class Cancellation
    {
        public int Category { get; set; }

        public int Days { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
