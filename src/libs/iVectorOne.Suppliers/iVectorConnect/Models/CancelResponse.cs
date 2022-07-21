namespace iVectorOne.Suppliers.iVectorConnect.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("CancelResponse")]
    public class CancelResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();
    }
}
