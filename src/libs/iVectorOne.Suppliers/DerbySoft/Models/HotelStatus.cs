﻿namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using System.Runtime.Serialization;

    public enum HotelStatus
    {
        [EnumMember(Value = "Actived")]
        Active,

        [EnumMember(Value = "Deactived ")]
        Inactive
    }
}