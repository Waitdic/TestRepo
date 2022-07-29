namespace iVectorOne.Suppliers.Restel
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Security;
    using Newtonsoft.Json;

    public class RestelThirdPartyReference
    {
        public string RoomCode { get; set; } = string.Empty;
        public string MealBasis { get; set; } = string.Empty;
        public List<string> ThirdPartyReferences { get; set; } = new();
        public string ResortCode { get; set; } = string.Empty;

        public string ToEncryptedString(ISecretKeeper secretKeeper)
        {
            return secretKeeper.Encrypt(JsonConvert.SerializeObject(this));
        }

        public static RestelThirdPartyReference FromEncryptedString(string trp, ISecretKeeper secretKeeper)
        {
            return JsonConvert.DeserializeObject<RestelThirdPartyReference>(secretKeeper.Decrypt(trp));
        }
    }
}
