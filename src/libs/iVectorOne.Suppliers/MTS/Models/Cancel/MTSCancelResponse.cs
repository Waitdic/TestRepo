namespace iVectorOne.Suppliers.MTS.Models.Cancel
{
    using System;
    using System.Xml.Serialization;
    using Common;


    [Serializable()]
    [XmlRoot("OTA_CancelRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSCancelResponse
    {
        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public string[] Errors { get; set; } = Array.Empty<string>();

        [XmlAttribute]
        public string Status { get; set; } = string.Empty;

        public UniqueID UniqueID { get; set; } = new();
    }
}
