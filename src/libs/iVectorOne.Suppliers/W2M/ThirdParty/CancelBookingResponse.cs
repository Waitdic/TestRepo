﻿using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "CancelBookingResponse")]
    public class CancelBookingResponse
    {
        [XmlElement(ElementName = "BookingRS")]
        public CancelBookingRS BookingRS { get; set; }
    }
}
