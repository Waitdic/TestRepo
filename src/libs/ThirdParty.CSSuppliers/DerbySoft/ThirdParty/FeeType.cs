namespace ThirdPartyInterfaces.DerbySoft.ThirdParty
{
    using System.Runtime.Serialization;

    public enum FeeType
    {
        [EnumMember(Value = "Inclusive")]
        Inclusive,

        [EnumMember(Value = "Exclusive")]
        Exclusive
    }
}
