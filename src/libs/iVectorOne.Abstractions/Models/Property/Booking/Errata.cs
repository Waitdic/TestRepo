namespace iVectorOne.Models.Property.Booking
{
    using System.Collections.Generic;

    /// <summary>
    /// A list of errata
    /// </summary>
    /// <seealso cref="List{Erratum}" />
    public class Errata : List<Erratum>
    {
        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        public void AddNew(string title, string text, string type)
        {
            Erratum erratum = new Erratum();
            {
                erratum.Title = title;
                erratum.Text = text;
                erratum.Type = type;
            }

            this.Add(erratum);
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="text">The text.</param>
        public void AddNew(string title, string text)
        {
            this.AddNew(title, text, string.Empty);
        }
    }
}
