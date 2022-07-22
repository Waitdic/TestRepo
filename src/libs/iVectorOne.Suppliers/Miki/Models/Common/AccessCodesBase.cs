namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class AccessCodesBase
    {
        [XmlElement("agentCode")]
        public string AgentCode { get; set; } = string.Empty;

        [XmlArray("dailyPasswords")]
        [XmlArrayItem("dailyPassword")]
        public DailyPassword[] DailyPasswords { get; set; } = Array.Empty<DailyPassword>();
    }
}
