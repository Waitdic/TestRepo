namespace ThirdParty.Models
{
    using System.Collections.Generic;
    using System.Xml;
    using Intuitive.Net.WebRequests;

    /// <summary>
    /// A list of logs
    /// </summary>
    /// <seealso cref="List{Log}" />
    public class Logs : List<Log>
    {
        /// <summary>
        /// Adds the specified web request XML logs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="request">The web request.</param>
        public void Add(string source, string title, Request request)
        {
            this.AddNew(source, title, request.RequestXML.OuterXml, request.ResponseXML.OuterXml);
        }

        /// <summary>
        /// Adds the specified request and response XMLs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="request">The request XML.</param>
        /// <param name="response">The response XML.</param>
        public void Add(string source, string title, XmlDocument request, XmlDocument response)
        {
            this.AddNew(source, title, request.OuterXml, response.OuterXml);
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="text">The text.</param>
        public void AddNew(string source, string title, string text)
        {
            var log = new Log()
            {
                Source = source,
                Text = text,
                Title = title,
            };

            this.Add(log);
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="xML">The XML.</param>
        public void AddNew(string source, string title, XmlDocument xML)
        {
            this.AddNew(source, title, xML.OuterXml);
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public void AddNew(string source, string title, string request, string response)
        {
            var requestlog = new Log()
            {
                Source = source,
                Text = request,
                Title = title + " Request",
            };

            this.Add(requestlog);

            var responselog = new Log()
            {
                Source = source,
                Text = response,
                Title = title + " Response",
            };

            this.Add(responselog);
        }
    }
}