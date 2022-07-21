namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Remark
    {
        [XmlArray("collects")]
        [XmlArrayItem("collect")]
        public List<Collect> Collects { get; set; } = new();
    }
}
