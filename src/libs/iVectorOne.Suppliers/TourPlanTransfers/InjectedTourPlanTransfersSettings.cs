namespace iVectorOne.Suppliers
{
    using Intuitive.Helpers.Extensions;
    using System.Collections.Generic;

    public class InjectedTourPlanTransfersSettings : ITourPlanTransfersSettings
    {
        public string URL { get; set; }
        public string AgentId { get; set; }
        public string Password { get; set; }
        public bool AllowCancellation { get; set; }

        public bool SetAgentId(Dictionary<string, string> thirdPartySettings)
        {
            if (GetValue("AgentId", thirdPartySettings,out var value))
            {
                AgentId = value;
                return true;
            }
            return false;
        }

        public bool SetURL(Dictionary<string, string> thirdPartySettings)
        {
            if (GetValue("URL", thirdPartySettings, out var value))
            {
                URL = value;
                return true;
            }
            return false;
        }

        public bool SetPassword(Dictionary<string, string> thirdPartySettings)
        {
            if (GetValue("Password", thirdPartySettings, out var value))
            {
                Password = value;
                return true;
            }
            return false;
        }

        public bool SetAllowCancellation(Dictionary<string, string> thirdPartySettings)
        {
            if (GetValue("SupportsLiveCancellations", thirdPartySettings, out var value))
            {
                AllowCancellation = value.ToSafeBoolean();
                return true;
            }
            return false;
        }

        private bool GetValue(string key, Dictionary<string, string> thirdPartySettings, out string value)
        {
            thirdPartySettings.TryGetValue(key, out value);
            if(!string.IsNullOrEmpty(value))
            {
                return true;
            }
            return false;
        }
    }
}