namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System;

    public class RoomProperty
    {
        public int Id { get; set; }

        public RoomType[] RoomTypes { get; set; } = Array.Empty<RoomType>();
    }
}
