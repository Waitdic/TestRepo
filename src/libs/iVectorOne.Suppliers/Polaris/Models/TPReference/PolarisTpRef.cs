using Intuitive.Helpers.Security;
using Newtonsoft.Json;

namespace iVectorOne.Suppliers.Polaris.Models
{
    internal class PolarisTpRef 
    {
        public string Encrypt(ISecretKeeper secretKeeper) 
        {
            return secretKeeper.Encrypt(JsonConvert.SerializeObject(this));
        }

        public static PolarisTpRef Decrypt(ISecretKeeper secretKeeper, string encTpRef) 
        {
            return JsonConvert.DeserializeObject<PolarisTpRef>(secretKeeper.Decrypt(encTpRef));
        }

        public string BookToken { get; set; } = string.Empty;
        public int RoomIndex { get; set; }
    }
}
