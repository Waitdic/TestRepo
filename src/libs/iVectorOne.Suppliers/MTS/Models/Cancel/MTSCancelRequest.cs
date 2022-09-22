namespace iVectorOne.Suppliers.MTS.Models.Cancel
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [Serializable()]
    [XmlRoot("OTA_CancelRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSCancelRequest
    {
        [XmlAttribute]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute]
        public string CancelType { get; set; } = string.Empty;

        public POS POS { get; set; } = new();

        public UniqueID UniqueID { get; set; } = new();
    }
}
