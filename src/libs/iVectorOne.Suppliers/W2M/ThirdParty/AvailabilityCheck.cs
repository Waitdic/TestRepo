﻿using System.Xml.Serialization;
using iVectorOne.Suppliers.Xml.W2M;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelCheckAvail")]
    public class AvailabilityCheck
    {
        public AvailabilityCheck(AvailabilityRequest availabilityRequest)
        {
            AvailabilityRequest = availabilityRequest;
        }

        public AvailabilityCheck()
        {
        }

        [XmlElement(ElementName = "HotelCheckAvailRQ")]
        public AvailabilityRequest AvailabilityRequest { get; set; }
    }
}
