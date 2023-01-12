namespace iVectorOne.Models.Models.Property
{
    /// <summary>Lookup Entity for Meal basis</summary>
    public class MealBasis
    {
        /// <summary>
        /// Gets or sets the meal basis identifier.
        /// </summary>
        public int MealBasisID { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meal basis code.
        /// </summary>
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the meal basis.
        /// </summary>
        public string MealBasisName { get; set; } = string.Empty;
    }
}