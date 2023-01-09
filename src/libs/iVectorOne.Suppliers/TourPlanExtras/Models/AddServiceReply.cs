using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.Suppliers.TourPlanExtras.Models
{
    public class AddServiceReply
    {
        public int BookingId { get; set; }
        public string Ref { get; set; }
        public int ServiceLineId { get; set; }
        public int SequenceNumber { get; set; }
        public string Status { get; set; }
        public Services Services { get; set; }
    }

    public class Services
    {
        public Service Service { get; set; }
    }

    public class Service
    {
        public string ServiceLineId { get; set; }
        public int ServiceLineUpdateCount { get; set; }
        public string Opt { get; set; }
        public string OptionNumber { get; set; }
        public string Date { get; set; }
        public string SequenceNumber { get; set; }
        public RoomConfigurations RoomConfigs { get; set; }
        public string CostedInBooking { get; set; }
        public decimal LinePrice { get; set; }
        public string SCU { get; set; }
        public int SCUqty { get; set; }
        public string puTime { get; set; }
        public string puRemark { get; set; }
        public string doTime { get; set; }
        public string doRemark { get; set; }
        public string Remarks { get; set; }
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string CancelDeleteStatus { get; set; }
        public string CanUpdate { get; set; }
        public string CanAccept { get; set; }
        public string LocationCode { get; set; }
        public string Status { get; set; }
        public CancelPolicies CancelPolicies { get; set; }
    }

    [XmlRoot(ElementName = "CancelPolicies")]
    public class CancelPolicies
    {
        [XmlElement(ElementName = "CancelPenalty")]
        public List<CancelPenalties> CancelPenalty { get; set; }
    }

    public class CancelPenalties
    {
        public Deadlines Deadline { get; set; }
        public string InEffect { get; set; }
        public string LinePrice { get; set; }
    }

    public class Deadlines
    {
        public string OffsetUnitMultiplier { get; set; }
        public string OffsetTimeUnit { get; set; }
        public string DeadlineDateTime { get; set; }
    }
}
