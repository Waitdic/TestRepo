namespace ThirdParty.CSSuppliers.ChannelManager.Models
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.ChannelManager.Models.Common;

    public class CancelResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        public string CancellationReference { get; set; } = string.Empty;
        [XmlArrayItem("BookingXML")]
        public List<XmlDocument> BookingXMLs { get; set; } = new();

        public string SupplierReference { get; set; } = string.Empty;

        [XmlArrayItem("TaskListItemDescription")]
        public List<string> TaskListItemDescriptions { get; set; } = new();
    }
}