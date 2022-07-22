namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class UniqueId
    {
        [XmlAttribute("ID_Context")]
        public string IdContext { get; set; } = string.Empty;
        public bool ShouldSerializeIdContext() => !string.IsNullOrEmpty(IdContext);

        [XmlAttribute("Type")]
        public string IdType { get; set; } = string.Empty;
        public bool ShouldSerializeIdType() => !string.IsNullOrEmpty(IdType);

        [XmlAttribute("ID")]
        public string Id { get; set; } = string.Empty;
        public bool ShouldSerializeId() => !string.IsNullOrEmpty(Id);
    }
}