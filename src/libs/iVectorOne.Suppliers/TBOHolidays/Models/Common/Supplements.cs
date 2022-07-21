namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Supplements
    {
        [XmlElement("SuppInfo")]
        public SuppInfo[] SuppInfo { get; set; } = Array.Empty<SuppInfo>();

        [XmlElement("Supplement")]
        public Supplement[] Supplement { get; set; } = Array.Empty<Supplement>();
    }
}
