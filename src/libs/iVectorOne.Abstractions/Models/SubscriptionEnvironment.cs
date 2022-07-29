namespace iVectorOne.Models
{
    public enum SubscriptionEnvironment
    {
        /// <summary>
        /// The subscription is for a test environment, only test supplier accounts should be used
        /// </summary>
        Test,

        /// <summary>
        /// The subscription is for a test environment, only live supplier accounts should be used.
        /// Bookings made with live subscriptions will be considered live with the supplier and will be charged.
        /// </summary>
        Live,
    }
}