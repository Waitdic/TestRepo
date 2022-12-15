﻿namespace iVectorOne.Suppliers
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Models;
    using System.Collections.Generic;

    public class InjectedTourPlanTransfersSettings : ITourPlanTransfersSettings
    {
        public string URL { get; set; }
        public string AgentId { get; set; }
        public string Password { get; set; }
        public bool AllowCancellation { get; set; }

        private Warnings Warnings;
        private readonly Warning ThirdPartySettingException = new Warning("ThirdPartySettingException", "The Third Party Setting: {0} must be provided.");

        public bool SetThirdPartySettings(Dictionary<string, string> thirdPartySettings)
        {
            AgentId = GetValue("AgentId", thirdPartySettings);
            Password = GetValue("Password", thirdPartySettings);
            URL = GetValue("URL", thirdPartySettings);
            AllowCancellation = GetValue("SupportsLiveCancellations", thirdPartySettings).ToSafeBoolean();
            return Validate();
        }

        public Warnings GetWarnings()
        {
            return Warnings;
        }

        private bool Validate()
        {
            Warnings = new();
            if (string.IsNullOrEmpty(AgentId))
                Warnings.AddNew(ThirdPartySettingException.Title, string.Format(ThirdPartySettingException.Text, nameof(AgentId)));

            if (string.IsNullOrEmpty(Password))
                Warnings.AddNew(ThirdPartySettingException.Title, string.Format(ThirdPartySettingException.Text, nameof(Password)));

            if (string.IsNullOrEmpty(URL))
                Warnings.AddNew(ThirdPartySettingException.Title, string.Format(ThirdPartySettingException.Text, nameof(URL)));

            if (Warnings.Count > 0)
                return false;
            return true;
        }

        private string GetValue(string key, Dictionary<string, string> thirdPartySettings)
        {
            thirdPartySettings.TryGetValue(key, out var value);
            return value;
        }
    }
}