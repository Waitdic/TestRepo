using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace iVectorOne.Search.Results.Models.Extra
{
    /// <summary>
    /// Transformed Extra Result Collection
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
        /// Gets or sets the transformed Extra results.
        /// </summary>
        /// <value>
        /// The transformed Extra results.
        /// </value>
        [XmlIgnore]
        public List<TransformedExtraResult> TransformedResults { get; set; } = new List<TransformedExtraResult>();

        /// <summary>
        /// Gets the valid extra results.
        /// </summary>
        /// <value>
        /// The valid extra results.
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
