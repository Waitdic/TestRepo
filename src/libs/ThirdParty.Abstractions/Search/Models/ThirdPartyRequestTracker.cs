namespace ThirdParty
{
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Serialization;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using ThirdParty.Search.Models;

    /// <summary>
    /// Class for tracking third party requests
    /// </summary>
    public class ThirdPartyRequestTracker : IRequestTracker
    {
        private readonly ISerializer _serializer;
        private readonly ISql _sql;

        public ThirdPartyRequestTracker(ISerializer serializer, ISql sql)
        {
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <summary>
        /// Gets or sets the requests sent.
        /// </summary>
        /// <value>
        /// The requests sent.
        /// </value>
        public int RequestsSent { get; set; } = 0;

        /// <summary>
        /// Gets or sets the requests received.
        /// </summary>
        /// <value>
        /// The requests received.
        /// </value>
        public int RequestsReceived { get; set; } = 0;

        /// <summary>
        /// Gets or sets the search id
        /// </summary>
        public string SearchID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request times.
        /// </summary>
        /// <value>
        /// The request times.
        /// </value>
        [XmlArray("RequestTimes")]
        [XmlArrayItem("RequestTime")]
        public List<ThirdPartyRequestTime> RequestTimes { get; set; } = new List<ThirdPartyRequestTime>();

        /// <summary>
        /// Store requests in db
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task StoreRequestTimesAsync()
        {
            var timeXML = _serializer.Serialize(RequestTimes);
            await _sql.ExecuteAsync(
                "StoreSearch",
                new CommandSettings()
                    .IsStoredProcedure()
                    .WithParameters(new
                    {
                        sSearchID = SearchID,
                        sRequestTimeXML = timeXML.InnerXml,
                    }));
        }
    }
}