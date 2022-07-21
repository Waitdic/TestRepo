﻿namespace iVectorOne.CSSuppliers.DerbySoft.Models
{
    using System.Runtime.Serialization;

    public enum ChargeType
    {
        [EnumMember(Value = "PerRoomPerNight")]
        PerRoomPerNight,

        [EnumMember(Value = "PerPersonPerNight")]
        PerPersonPerNight,

        [EnumMember(Value = "PerRoomPerStay")]
        PerRoomPerStay,

        [EnumMember(Value = "PerPersonPerStay")]
        PerPersonPerStay
    }
}