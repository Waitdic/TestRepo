namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class PackageDeleteResponse : SoapContent
    {
        [XmlElement("PACKAGEDELETEResult")]
        public PackageDeleteResult Response { get; set; }
    }
}
