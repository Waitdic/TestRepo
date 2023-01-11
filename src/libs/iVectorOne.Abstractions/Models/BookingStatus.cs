namespace iVectorOne.Models
{
    public enum BookingStatus
    {
        /// <summary>
        /// The booking request was invalid, no attempt to book with the supplier has been made
        /// </summary>
        Invalid,

        /// <summary>
        /// The booking was made successfully and has not been cancelled
        /// </summary>
        Live,

        /// <summary>
        /// The booking failed either with the supplier or in post-processing in IVO
        /// </summary>
        Failed,

        /// <summary>
        /// The booking has been cancelled successfully
        /// </summary>
        Cancelled,
    }
}