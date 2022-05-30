﻿namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class PosSource
    {
        public PosSource() { }
        [XmlAttribute("AgentDutyCode")]
        public string AgentDutyCode { get; set; } = string.Empty;

        [XmlElement("RequestorID")]
        public RequestorId RequestorId { get; set; } = new();
    }
}