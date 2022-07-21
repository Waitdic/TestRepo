namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OTA_CancelRS
    {
        [XmlElement("Success")]
        public string sSuccess { get; set; } = Constant.NotEmptyStringToken;

        public bool Success { get => string.IsNullOrEmpty(sSuccess); }

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<ErrorType> Errors { get; set; } = new();
    }
}