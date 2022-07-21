namespace ThirdParty.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A list of logs
    /// </summary>
    /// <seealso cref="List{Log}" />
    public class Logs : List<Log>
    {
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