namespace iVectorOne.CSSuppliers.DerbySoft.Models
{
    using System.Runtime.Serialization;

    public enum FeeAmountType
    {
        [EnumMember(Value = "Fix")]
        Fix,

        [EnumMember(Value = "Percent")]
        Percent
    }
}