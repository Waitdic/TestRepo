namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class NightPrice
    {
        public float Amount { get; set; }

        [XmlArray("TaxesAndFees")]
        [XmlArrayItem("TaxAndFee")]
        public List<TaxAndFee> TaxesAndFees { get; set; } = new();
    }
}
