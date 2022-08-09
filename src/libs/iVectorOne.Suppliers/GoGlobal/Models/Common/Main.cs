using System.Xml.Serialization;

namespace iVectorOne.Suppliers.GoGlobal.Models
{
    public abstract class Main
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;
        public bool ShouldSerializeVersion() => !string.IsNullOrEmpty(Version);

        [XmlAttribute("ResponseFormat")]
        public string ResponseFormat { get; set; } = string.Empty;
        public bool ShouldSerializeResponseFormat() => !string.IsNullOrEmpty(ResponseFormat);

        public string Error { get; set; } = string.Empty;
        public bool ShouldSerializeError() => !string.IsNullOrEmpty(Error);

        public string DebugError { get; set; } = string.Empty;
        public bool ShouldSerializeDebugError() => !string.IsNullOrEmpty(DebugError);
    }
}
