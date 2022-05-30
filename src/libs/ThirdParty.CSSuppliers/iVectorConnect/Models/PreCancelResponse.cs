namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PreCancelResponse")]
    public class PreCancelResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        public decimal CancellationCost { get; set; }

        public string CancellationToken { get; set; } = string.Empty;
    }
}
