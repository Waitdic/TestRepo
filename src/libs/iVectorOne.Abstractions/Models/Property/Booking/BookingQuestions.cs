namespace iVectorOne.Models.Property.Booking
{
    using System.Collections.Generic;
    using iVectorOne.Models;

    /// <summary>
    /// A booking question
    /// </summary>
    public class BookingQuestion
    {
        /// <summary>
        /// Gets or sets the question
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the answer
        /// </summary>
        public string Answer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BookingQuestion"/> is mandatory.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the third party reference.
        /// </summary>
        public string ThirdPartyReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public List<Option> Options { get; set; } = new();
    }
}