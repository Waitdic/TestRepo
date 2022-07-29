namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public abstract class BaseRequest
    {
        public static string Version4 = "4.0";
        private const string DefaultLanguage = "ENG";

        protected BaseRequest()
        {
        }

        protected BaseRequest(string version, string clientCode, string password, string branch, string tradingGroup,
            string languageCode)
        {
            Version = string.IsNullOrEmpty(version) ? Version4 : version;
            Language.Code = string.IsNullOrEmpty(languageCode) ? DefaultLanguage : languageCode;

            Client.Code = clientCode;
            Client.Branch = branch;
            Client.TradingGroup = tradingGroup;

            if (Version != Version4)
            {
                Client.Password = password;
            }
        }

        [XmlAttribute("type")]
        public abstract string Type { get; set; }

        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlElement("client")]
        public Client Client { get; set; } = new();

        [XmlElement("language")]
        public Language Language { get; set; } = new();
    }
}
