﻿namespace iVectorOne.Suppliers
{
    using iVectorOne.Models;
    using System.Collections.Generic;
    public interface ITourPlanExtrasSettings
    {
        string URL { get; set; }
        string AgentId { get; set; }
        string Password { get; set; }
        bool AllowCancellation { get; set; }
        bool SetThirdPartySettings(Dictionary<string, string> thirdPartySettings);
        Warnings GetWarnings();
    }
}
