namespace iVectorOne.Models.Property.Booking
{
    /// <summary>
    /// A booking comment
    /// </summary>
    public class BookingComment
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the booking comment.
        /// </summary>
        /// <value>
        /// The type of the booking comment.
        /// </value>
        public string BookingCommentType { get; set; } = string.Empty;
    }
}
