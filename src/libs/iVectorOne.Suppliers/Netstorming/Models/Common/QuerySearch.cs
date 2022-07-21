namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class QuerySearch
    {
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Number)
                   && string.IsNullOrEmpty(Agreement)
                   && string.IsNullOrEmpty(Price);
        }

        [XmlAttribute("number")]
        public string Number { get; set; } = string.Empty;

        [XmlAttribute("agreement")]
        public string Agreement { get; set; } = string.Empty;

        [XmlAttribute("price")]
        public string Price { get; set; } = string.Empty;
    }
}