namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ResGuest
    {
        public ResGuest() { }

        [XmlArray("Profiles")]
        [XmlArrayItem("ProfileInfo")]
        public List<ProfileInfo> Profiles { get; set; } = new();
    }
}