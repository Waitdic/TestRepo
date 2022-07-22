namespace iVectorOne.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A single result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Result"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Gets or sets the reference
        /// </summary>
        public string Reference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the line
        /// </summary>
        public string Line { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The logs
        /// </summary>
        public List<Log> Logs { get; set; } = new List<Log>();
    }
}
