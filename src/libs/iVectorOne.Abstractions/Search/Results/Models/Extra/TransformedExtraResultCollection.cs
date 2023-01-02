namespace iVectorOne.Search.Results.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Transformed Transfer Result Collection
    /// </summary>
    [XmlRoot("Results")]
    public class TransformedExtraResultCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformedExtraResultCollection"/> class.
        /// </summary>
        public TransformedExtraResultCollection()
        {
        }

        /// <summary>
        /// Gets or sets the transformed transfer results.
        /// </summary>
        /// <value>
        /// The transformed transfer results.
        /// </value>
        [XmlIgnore]
        public List<TransformedExtraResult> TransformedResults { get; set; } = new List<TransformedExtraResult>();

        /// <summary>
        /// Gets the valid transfer results.
        /// </summary>
        /// <value>
        /// The valid transfer results.
        /// </value>
        [XmlElement("Result")]
        public List<TransformedExtraResult> ValidResults
        {
            get
            {
                return this.TransformedResults.Where(x => x.Warnings.Count == 0).ToList();
            }
        }
    }
}
