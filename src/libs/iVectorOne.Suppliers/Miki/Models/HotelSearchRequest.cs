﻿namespace iVectorOne.Suppliers.Miki.Models
{
    using System.Xml.Serialization;
    using Common;

    public class HotelSearchRequest : SoapContent
    {
        [XmlAttribute("versionNumber")]
        public string VersionNumber { get; set; } = string.Empty;

        [XmlElement("requestAuditInfo")]
        public RequestAuditInfo RequestAuditInfo { get; set; } = new();

        [XmlElement("hotelSearchCriteria")]
        public HotelSearchCriteria HotelSearchCriteria { get; set; } = new();
    }
}
