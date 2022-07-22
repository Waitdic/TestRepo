namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Header
{
    public class AmaSecurityHostedUser
    {
        public UserID? UserID { get; set; }
        public bool ShouldSerializeUserID() => UserID != null;
    }
}
