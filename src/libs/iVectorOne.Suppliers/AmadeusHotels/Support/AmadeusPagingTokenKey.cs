namespace iVectorOne.CSSuppliers.AmadeusHotels.Support
{
    using System.Linq;
    using iVectorOne.Models;
    using iVectorOne.Interfaces;

    public class AmadeusPagingTokenKey : IPagingTokenKey
    {
        public ResortSplit ResortSplit { get; }

        public int RoomId { get; }

        public AmadeusPagingTokenKey(ResortSplit resortSplit, int roomId)
        {
            ResortSplit = resortSplit;
            RoomId = roomId;
        }

        public override bool Equals(object obj)
        {
            return Equals((AmadeusPagingTokenKey)obj);
        }

        public bool Equals(AmadeusPagingTokenKey? obj)
        {
            return obj != null && obj.ResortSplit.Equals(ResortSplit) && obj.RoomId.Equals(RoomId);
        }

        public override int GetHashCode()
        {
            return (ResortSplit.ResortCode + ResortSplit.ThirdPartySupplier + string.Join(",", ResortSplit.Hotels.Select(hotel => hotel.TPKey)) + RoomId).GetHashCode();
        }
    }
}