namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive.Helpers.Extensions;
    using System;
    using System.Collections.Generic;

    public class TourPlanHelper
    {
        public static Tuple<bool, string, ITourPlanTransfersSettings> SetThirdPartySettings(Dictionary<string, string> thirdPartySettings)
        {
            thirdPartySettings.TryGetValue("AgentId", out var agentId);
            thirdPartySettings.TryGetValue("Password", out var password);
            thirdPartySettings.TryGetValue("URL", out var url);
            thirdPartySettings.TryGetValue("SupportsLiveCancellations", out var supportsLiveCancellations);

            if (string.IsNullOrEmpty(agentId))
            {
                return new Tuple<bool, string, ITourPlanTransfersSettings>(false, "AgentId", null);
            }
            if (string.IsNullOrEmpty(password))
            {
                return new Tuple<bool, string, ITourPlanTransfersSettings>(false, "Password", null);
            }
            if (string.IsNullOrEmpty(url))
            {
                return new Tuple<bool, string, ITourPlanTransfersSettings>(false, "Url", null);
            }

            var setting = new InjectedTourPlanTransfersSettings
            {
                URL = url,
                Password = password,
                AgentId = agentId,
                AllowCancellation = supportsLiveCancellations.ToSafeBoolean()
            };
            return new Tuple<bool, string, ITourPlanTransfersSettings>(true, string.Empty, setting);
        }
    }
}
