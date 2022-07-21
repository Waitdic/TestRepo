namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class QueryHotel
    {
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Id)
                   && string.IsNullOrEmpty(Value)
                   && string.IsNullOrEmpty(Code)
                   && string.IsNullOrEmpty(Agreement);
        }

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlText]
        public string Value { get; set; } = string.Empty;

        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("agreement")]
        public string Agreement { get; set; } = string.Empty;
    }
}