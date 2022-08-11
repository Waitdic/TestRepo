namespace iVectorOne.Search.Results.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Transformed Result Collection
    /// </summary>
    [XmlRoot("Results")]
    public class TransformedResultCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformedResultCollection"/> class.
        /// </summary>
        public TransformedResultCollection()
        {
            this.EqualityComparer = new TransformedResultsComparer();
        }

        /// <summary>
        /// Gets or sets the transformed results.
        /// </summary>
        [XmlIgnore]
        public List<TransformedResult> TransformedResults { get; set; } = new List<TransformedResult>();

        /// <summary>
        /// Gets the distinct valid results.
        /// </summary>
        [XmlElement("Result")]
        public List<TransformedResult> DistinctValidResults
        {
            get
            {
                return this.TransformedResults.Where(x => x.Warnings.Count == 0).Distinct(this.EqualityComparer).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the equality comparer.
        /// </summary>
        private IEqualityComparer<TransformedResult> EqualityComparer { get; set; }
    }
}