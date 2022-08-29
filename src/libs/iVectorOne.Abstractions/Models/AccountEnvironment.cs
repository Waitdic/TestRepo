namespace iVectorOne.Models
{
    public enum AccountEnvironment
    {
        /// <summary>
        /// The account is for a test environment, only test supplier accounts should be used
        /// </summary>
        Test,

        /// <summary>
        /// The account is for a test environment, only live supplier accounts should be used.
        /// Bookings made with live accounts will be considered live with the supplier and will be charged.
        /// </summary>
        Live,
    }
}