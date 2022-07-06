namespace ThirdParty.CSSuppliers.TBOHolidays.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class HotelCancellationPolicyResponse : SoapContent
    {
        public Status Status { get; set; } = new();

        [XmlArray("CancelPolicies")]
        [XmlArrayItem("CancelPolicy", Type = typeof(CancelPolicy))]
        [XmlArrayItem("DefaultPolicy", Type = typeof(DefaultPolicy))]
        public BasePolicy[] CancelPolicies { get; set; } = Array.Empty<BasePolicy>();

        [XmlArray("HotelNorms")]
        public string[] HotelNorms { get; set; } = Array.Empty<string>();
    }
}
