using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.TourPlanExtras.Models
{
    public class CancelServicesReply
    {
        public int BookingId { get; set; }
        public string Ref { get; set; }
        public ServiceStatuses ServiceStatuses { get; set; }
    }
    public class ServiceStatuses
    {
        public ServiceStatusContents ServiceStatus { get; set; }
    }
    public class ServiceStatusContents {
        public string ServiceLineID { get; set; }
        public string Date { get; set; }
        public string SequenceNumber { get; set; }
        public string Status { get; set; }
    }
}
