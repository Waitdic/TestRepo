namespace iVectorOne.Suppliers.PremierInn.Models
{
    using Intuitive.Helpers.Security;
    using Newtonsoft.Json;
    using iVectorOne.Suppliers.PremierInn.Models.Common;

    public class PremierInnTpRef
    {
        public PremierInnTpRef(string sessionId, string rateCode, CancellationPolicy cancellation)
        {
            SessionId = sessionId;
            RateCode = rateCode;
            CancellationPolicy = new()
            {
                Category = cancellation.Category,
                Days = cancellation.Days,
                Text = cancellation.Text
            };
        }

        public PremierInnTpRef()
        {

        }

        public string SessionId { get; set; } = string.Empty;

        public string RateCode { get; set; } = string.Empty;

        public Cancellation CancellationPolicy { get; set; } = new();

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
