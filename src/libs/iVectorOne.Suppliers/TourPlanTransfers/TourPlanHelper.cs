namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive.Helpers.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class TourPlanHelper
    {
        public static Tuple<bool, string> SetThirdPartySettings(ITourPlanTransfersSettings settings, Dictionary<string, string> thirdPartySettings)
        {
            if(thirdPartySettings.Count == 0)
            {
                return new Tuple<bool, string>(false, nameof(ITourPlanTransfersSettings.AgentId));
            }
            if(!settings.SetAgentId(thirdPartySettings))
            {
                return new Tuple<bool, string>(false, nameof(ITourPlanTransfersSettings.AgentId));
            }
            if (!settings.SetPassword(thirdPartySettings))
            {
                return new Tuple<bool, string>(false, nameof(ITourPlanTransfersSettings.Password));
            }
            if (!settings.SetURL(thirdPartySettings))
            {
                return new Tuple<bool, string>(false, nameof(ITourPlanTransfersSettings.URL));
            }

            settings.SetAllowCancellation(thirdPartySettings);

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
