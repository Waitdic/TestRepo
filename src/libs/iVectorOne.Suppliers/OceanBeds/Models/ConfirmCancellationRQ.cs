namespace ThirdParty.CSSuppliers.OceanBeds.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("ConfirmCancellationRQ", Namespace = "http://oceanbeds.com/2014/10")]
    public class ConfirmCancellationRQ
    {
        public Credential Credential { get; set; } = new();

        public int Key { get; set; }
    }
}
