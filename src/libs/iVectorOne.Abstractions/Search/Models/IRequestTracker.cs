namespace iVectorOne.Search.Models
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRequestTracker
    {
        /// <summary>
        /// Gets or sets the requests sent.
        /// </summary>
        /// <value>
        /// The requests sent.
        /// </value>
        public int RequestsSent { get; set; }

        /// <summary>
        /// Gets or sets the requests received.
        /// </summary>
        /// <value>
        /// The requests received.
        /// </value>
        public int RequestsReceived { get; set; }

        /// <summary>
        /// Gets or sets the search id
        /// </summary>
        public string SearchID { get; set; }

        /// <summary>
        /// Gets or sets the request times.
        /// </summary>
        /// <value>
        /// The request times.
        /// </value>
        public List<ThirdPartyRequestTime> RequestTimes { get; set; }

        /// <summary>
        /// Stores the request times
        /// </summary>
        public Task StoreRequestTimesAsync();
    }
}
