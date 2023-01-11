using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.TourPlanExtras.Models
{
    public class AddServiceRequest
    {
        public string AgentID { get; set; }
        public string Password { get; set; }
        public string Opt { get; set; }
        public string ExistingBookingInfo { get; set; }
        public string RateId { get; set; }
        public string DateFrom { get; set; }
        public NewBookingInformation NewBookingInfo { get; set; }
        public RoomConfigurations RoomConfigs { get; set; }
        public string PickUp_Date { get; set; }
        public string PuTime { get; set; }
        public string PuRemark { get; set; }
    }

    public class NewBookingInformation
    {
        public string Name { get; set; }
        public string QB { get; set; } = "B";
    }

    public class RoomConfigurations
    {
        public RoomConfiguration RoomConfig { get; set; }
    }
    public class RoomConfiguration
    {
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public List<PaxDetails> PaxList { get; set; } = new List<PaxDetails>();
    }
    public class PaxDetails
    {
        public string Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string PaxType { get; set; }

    }
}
