using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.TourPlanTransfers.Models
{
    public class CancelServicesRequest
    {
        public string AgentID { get; set; }
        public string Password { get; set; }
        public string Ref { get; set; }
    }
}
