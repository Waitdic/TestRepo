namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelRefs
    {
        [XmlArray("productCodes")]
        [XmlArrayItem("productCode")]
        public string[] ProductCodes { get; set; } = Array.Empty<string>();
    }
}
