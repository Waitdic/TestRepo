using System;
using System.Collections.Generic;

namespace iVectorOne.Suppliers.TourPlanTransfers.Models
{
    public class OptionInfoRequest
    {
        public string AgentID { get; set; }
        public string Password { get; set; }
        public string Opt { get; set; }
        public string Info { get; set; }
        public string DateFrom { get; set; }
        public List<RoomConfiguration> RoomConfigs { get; set; }
    }
}
