namespace ThirdParty.Models
{
    /// <summary>Lookup Entity for Meal basis</summary>
    public class MealBasis
    {
        /// <summary>
        /// Gets or sets the meal basis identifier.
        /// </summary>
        /// <value>
        /// The meal basis identifier.
        /// </value>
        public int MealBasisID { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meal basis code.
        /// </summary>
        /// <value>
        /// The meal basis code.
        /// </value>
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the meal basis.
        /// </summary>
        /// <value>
        /// The name of the meal basis.
        /// </value>
        public string MealBasisName { get; set; } = string.Empty;
    }
}
