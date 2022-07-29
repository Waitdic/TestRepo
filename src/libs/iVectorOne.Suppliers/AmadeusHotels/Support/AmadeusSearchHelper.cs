namespace iVectorOne.Suppliers.AmadeusHotels.Support
{
    public class AmadeusSearchHelper
    {
        public AmadeusSearchHelper()
        {
        }

        public AmadeusSearchHelper(AmadeusPagingTokenKey pagingTokenKey, int propertyRoomBookingID, int adults, int children, int duration)
        {
            PagingTokenKey = pagingTokenKey;
            PropertyRoomBookingID = propertyRoomBookingID;
            Adults = adults;
            Children = children;
            Duration = duration;
        }

        public AmadeusPagingTokenKey PagingTokenKey { get; set; }

        public int PropertyRoomBookingID { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        public int Duration { get; set; }
    }
}
