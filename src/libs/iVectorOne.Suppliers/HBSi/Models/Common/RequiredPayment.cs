namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RequiredPayment
    {
        [XmlArray("AcceptedPayments")]
        [XmlArrayItem("AcceptedPayment")]
        public List<AcceptedPayment> AcceptedPayments { get; set; } = new();

        public AmountPercent AmountPercent { get; set; } = new();
        public Deadline Deadline { get; set; } = new();
    }
}
