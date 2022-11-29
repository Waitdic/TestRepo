using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.TourPlanTransfers.Models
{
    public class AddServiceRequest
    {
        public string AgentID { get; set; }
        public string Password { get; set; }
        public string Opt { get; set; }
        public string ExistingBookingInfo { get; set; }
        public string RateID { get; set; }
        public DateTime DateFrom { get; set; }
        public NewBookingInfo NewBookingInfo { get; set; }
        public RoomConfigs RoomConfigs { get; set; }
        public DateTime PickUp_Date { get; set; }
        public string PuTime { get; set; }
        public string PuRemark { get; set; }
    }

    public class NewBookingInfo
    {
        public string Name { get; set; }
        public string QB { get; set; } = "B";
    }

    public class RoomConfigs
    {
        public RoomConfig RoomConfig { get; set; }
    }
    public class RoomConfig
    {
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public PaxList PaxList { get; set; }
    }

    public class PaxList
    {
        public PaxDetails PaxDetails { get; set; }
    }

    public class PaxDetails
    {
        public string Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string PaxType { get; set; }

    }
}
