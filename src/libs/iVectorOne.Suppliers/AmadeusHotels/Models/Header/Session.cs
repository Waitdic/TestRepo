namespace iVectorOne.Suppliers.AmadeusHotels.Models.Header
{
    using System.Xml.Serialization;

    public class Session
    {
        [XmlAttribute("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;
        public bool ShouldSerializeSessionId() => !string.IsNullOrEmpty(SessionId);

        public int? SequenceNumber { get; set; }
        public bool ShouldSerializeSequenceNumber() => SequenceNumber != null;

        public string SecurityToken { get; set; } = string.Empty;
        public bool ShouldSerializeSecurityToken() => !string.IsNullOrEmpty(SecurityToken);
    }
}
