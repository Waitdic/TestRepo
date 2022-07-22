namespace iVectorOne.CSSuppliers.DerbySoft.Models
{
    using System.Runtime.Serialization;

    public enum CancelChargeBase
    {
        [EnumMember(Value = "FullStay")]
        FullStay,
        [EnumMember(Value = "NightBase")]
        NightBase,
        [EnumMember(Value = "Amount")]
        Amount
    }
}