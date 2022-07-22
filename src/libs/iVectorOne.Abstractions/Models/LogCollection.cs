namespace iVectorOne
{
    using System.Xml;

    /// <summary>
    /// A collection of logs
    /// </summary>
    public class LogCollection
    {     
        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="requestXML">The request XML.</param>
        /// <param name="responseXML">The response XML.</param>
        public LogCollection(string provider, XmlDocument requestXML, XmlDocument responseXML)
        {
            this.Provider = provider;
            this.RequestXML = requestXML;
            this.ResponseXML = responseXML;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestXML">The request XML.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, XmlDocument requestXML, XmlDocument responseXML, string responseTime, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestXML = requestXML;
            this.ResponseXML = responseXML;
            this.ResponseTime = responseTime;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestXML">The request XML.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="storeXML">The store XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, XmlDocument requestXML, XmlDocument responseXML, XmlDocument storeXML, string responseTime, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestXML = requestXML;
            this.ResponseXML = responseXML;
            this.StoreXML = storeXML;
            this.ResponseTime = responseTime;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestString">The request string.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, string requestString, XmlDocument responseXML, string responseTime, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestString = requestString;
            this.ResponseXML = responseXML;
            this.ResponseTime = responseTime;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestString">The request string.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, string requestString, string responseXML, string responseTime, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestString = requestString;
            XmlDocument xML = new XmlDocument();
            xML.InnerXml = responseXML;
            this.ResponseXML = xML;
            this.ResponseTime = responseTime;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestXML">The request XML.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="responseLog">The response log.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, XmlDocument requestXML, XmlDocument responseXML, string responseTime, string responseLog, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestXML = requestXML;
            this.ResponseXML = responseXML;
            this.ResponseTime = string.Empty;
            this.ResponseLog = responseLog;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestString">The request string.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="storeXML">The store XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, string requestString, XmlDocument responseXML, XmlDocument storeXML, string responseTime, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestString = requestString;
            this.ResponseXML = responseXML;
            this.StoreXML = storeXML;
            this.ResponseTime = responseTime;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestString">The request string.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="responseLog">The response log.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, string requestString, XmlDocument responseXML, string responseTime, string responseLog, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestString = requestString;
            this.ResponseXML = responseXML;
            this.ResponseTime = string.Empty;
            this.ResponseLog = responseLog;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCollection"/> class.
        /// </summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="requestString">The request string.</param>
        /// <param name="requestXML">The request XML.</param>
        /// <param name="responseLog">The response log.</param>
        /// <param name="responseXML">The response XML.</param>
        /// <param name="storeXML">The store XML.</param>
        /// <param name="responseTime">The response time.</param>
        /// <param name="resultsCount">The results count.</param>
        public LogCollection(string supplier, string requestString, XmlDocument requestXML, string responseLog, XmlDocument responseXML, XmlDocument storeXML, string responseTime, string resultsCount)
        {
            this.Supplier = supplier;
            this.RequestString = requestString;
            this.RequestXML = requestXML;
            this.ResponseLog = responseLog;
            this.ResponseXML = responseXML;
            this.StoreXML = storeXML;
            this.ResponseTime = responseTime;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        /// <value>
        /// The supplier.
        /// </value>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request XML.
        /// </summary>
        /// <value>
        /// The request XML.
        /// </value>
        public XmlDocument RequestXML { get; set; } = null!;

        /// <summary>
        /// Gets or sets the request string.
        /// </summary>
        /// <value>
        /// The request string.
        /// </value>
        public string RequestString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the response XML.
        /// </summary>
        /// <value>
        /// The response XML.
        /// </value>
        public XmlDocument ResponseXML { get; set; } = null!;

        /// <summary>
        /// Gets or sets the response string.
        /// </summary>
        /// <value>
        /// The response string.
        /// </value>
        public string ResponseString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the response time.
        /// </summary>
        /// <value>
        /// The response time.
        /// </value>
        public string ResponseTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the results count.
        /// </summary>
        /// <value>
        /// The results count.
        /// </value>
        public string ResultsCount { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the response log.
        /// </summary>
        /// <value>
        /// The response log.
        /// </value>
        public string ResponseLog { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the store XML.
        /// </summary>
        /// <value>
        /// The store XML.
        /// </value>
        public XmlDocument StoreXML { get; set; } = null!;
    }
}
