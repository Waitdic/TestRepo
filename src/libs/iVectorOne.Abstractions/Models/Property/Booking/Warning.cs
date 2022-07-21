namespace iVectorOne.Models.Property.Booking
{
    /// <summary>
    /// A class for warnings
    /// </summary>
    public class Warning
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Warning"/> class.
        /// </summary>
        public Warning()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Warning"/> class.
        /// </summary>
        /// <param name="title">The s title.</param>
        /// <param name="text">The s text.</param>
        public Warning(string title, string text)
        {
            this.Title = title;
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Warning"/> class.
        /// </summary>
        /// <param name="title">The s title.</param>
        /// <param name="text">The s text.</param>
        /// <param name="type">Type of the s.</param>
        public Warning(string title, string text, WarningType type) : this(title, text)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the The title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the The text
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the The type of warning
        /// </summary>
        public WarningType Type { get; set; } = WarningType.Exception;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Title}: {Text}";
        }
    }
}
