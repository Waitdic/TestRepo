namespace iVectorOne.Suppliers
{
    using System.Collections.Generic;
    public interface ITourPlanTransfersSettings
    {
        string URL { get; set; }
        string AgentId { get; set; }
        string Password { get; set; }
        bool AllowCancellation { get; set; }
        bool SetAgentId(Dictionary<string, string> thirdPartySettings);
        bool SetURL(Dictionary<string, string> thirdPartySettings);
        bool SetPassword(Dictionary<string, string> thirdPartySettings);
        bool SetAllowCancellation(Dictionary<string, string> thirdPartySettings);
    }
}
