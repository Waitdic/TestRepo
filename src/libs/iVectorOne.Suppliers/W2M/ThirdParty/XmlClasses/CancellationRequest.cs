using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "CancelRQ")]
    public class CancellationRequest
    {
        public CancellationRequest(Login login, CancelRequest cancelRequest, 
            CancellationAdvancedOptions cancellationAdvancedOptions, string version, string language)
        {
            Login = login;
            CancelRequest = cancelRequest;
            CancellationAdvancedOptions = cancellationAdvancedOptions;
            Version = version;
            Language = language;
        }

        public CancellationRequest()
        {
        }

        [XmlElement(ElementName = "Login")]
        public Login Login { get; set; }
        [XmlElement(ElementName = "CancelRequest")]
        public CancelRequest CancelRequest { get; set; }
        [XmlElement(ElementName = "AdvancedOptions")]
        public CancellationAdvancedOptions CancellationAdvancedOptions { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
    }
}
