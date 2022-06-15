namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    using System.Xml.Serialization;

    public partial class BookingLogin
    {

        public string UserName { get; set; }
        public string Password { get; set; }

        [XmlIgnore()]
        public bool LoggedIn { get; set; } = false;

        [XmlIgnore()]
        public string Warning { get; set; } = "";

    }
}