namespace ThirdParty.Models.Property.Booking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A list of Booking Comment
    /// </summary>
    /// <seealso cref="List{BookingComment}" />
    public class BookingComments : List<BookingComment>
    {
        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="commentType">Type of the comment.</param>
        public void AddNew(string text, string commentType = "")
        {
            var bookingComment = new BookingComment()
            {
                Text = text,
                BookingCommentType = commentType,
            };

            this.Add(bookingComment);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.Select(o => o.Text).ToList());
        }
    }
}
