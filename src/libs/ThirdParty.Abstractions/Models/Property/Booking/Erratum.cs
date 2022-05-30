namespace ThirdParty.Models.Property.Booking
{
    /// <summary>
    /// A single Erratum
    /// </summary>
    public class Erratum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Erratum"/> class.
        /// </summary>
        public Erratum()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Erratum"/> class.
        /// </summary>
        /// <param name="text">The s text.</param>
        public Erratum(string text)
        {
            this.Title = "Important Information";
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Erratum"/> class.
        /// </summary>
        /// <param name="title">The s title.</param>
        /// <param name="text">The s text.</param>
        public Erratum(string title, string text)
        {
            this.Title = title;
            this.Text = text;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; } = string.Empty;
    }
}