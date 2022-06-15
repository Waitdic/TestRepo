namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    public partial class CancelResponse
    {

        public ReturnStatus ReturnStatus { get; set; }

        public string CancellationReference { get; set; } = string.Empty;
        [XmlArrayItem("BookingXML")]
        public List<XmlDocument> BookingXMLs { get; set; } = new List<XmlDocument>();

        public string SupplierReference { get; set; } = "";

        [XmlArrayItem("TaskListItemDescription")]
        public List<string> TaskListItemDescriptions { get; set; } = new List<string>();

    }
}