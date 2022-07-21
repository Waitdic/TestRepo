namespace ThirdParty.Support
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// A class for masking data
    /// </summary>
    public class DataMasking
    {
        /// <summary>
        /// this one is used for direct, class create and use method
        /// </summary>
        /// <param name="xML">The x ml.</param>
        /// <param name="dataMasker">The data masker.</param>
        /// <returns>an XML document</returns>
        public static XmlDocument DocumentReplace(XmlDocument xML, DataMaskLookups dataMasker)
        {
            // return the XML passed in if there is nothing
            if (dataMasker == null)
            {
                return xML;
            }

            // replace each node that is requested to be replaced
            foreach (DataMaskLookup dataMask in dataMasker)
            {
                xML = DocumentReplace(xML, dataMask.XPath, dataMask.Regex, dataMasker.NameSpaceManager);
            }

            return xML;
        }

        /// <summary>
        /// this one is overloaded so that you can exclude the namespace manager
        /// </summary>
        /// <param name="xML">The x ml.</param>
        /// <param name="xPath">The x path.</param>
        /// <param name="regexPattern">The regex pattern.</param>
        /// <returns>An xml document</returns>
        public static XmlDocument DocumentReplace(XmlDocument xML, string xPath, string regexPattern)
        {
            return DocumentReplace(xML, xPath, regexPattern, null!);
        }

        /// <summary>
        /// This is the one which does the actual work
        /// </summary>
        /// <param name="xML">The x ml.</param>
        /// <param name="xPath">The x path.</param>
        /// <param name="regexPattern">The regex pattern.</param>
        /// <param name="nameSpaceManager">The name space manager.</param>
        /// <returns>an XML document</returns>
        /// <exception cref="Exception">empty xml passed to data masking replace method</exception>
        public static XmlDocument DocumentReplace(XmlDocument xML, string xPath, string regexPattern, XmlNamespaceManager nameSpaceManager)
        {
            try
            {
                if (xML == null)
                {
                    throw new Exception("empty xml passed to data masking replace method");
                }

                var nodes = xML.SelectNodes(xPath, nameSpaceManager);
                foreach (XmlNode node in nodes)
                {
                    string nodeValue = node.InnerText;
                    node.InnerText = Regex.Replace(nodeValue, regexPattern, new MatchEvaluator(DataMaskReplace), RegexOptions.Compiled);
                }
            }
            catch
            {
                // not that fussed
            }

            return xML;
        }

        /// <summary>
        /// Replaces the passed in match with X
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>a string</returns>
        public static string DataMaskReplace(Match match)
        {
            return string.Empty.PadLeft(match.Length, 'X');
        }

        /// <summary>
        /// A lookup for the data mask to use
        /// </summary>
        /// <seealso cref="List{DataMaskLookup}" />
        public class DataMaskLookups : List<DataMaskLookup>
        {
            /// <summary>
            /// Gets or sets the name space manager.
            /// </summary>
            /// <value>
            /// The name space manager.
            /// </value>
            public XmlNamespaceManager NameSpaceManager { get; set; } = null!;

            /// <summary>
            /// Adds the specified x path.
            /// </summary>
            /// <param name="xPath">The x path.</param>
            /// <param name="regex">The regex.</param>
            public void Add(string xPath, string regex)
            {
                var dataMaskLookup = new DataMaskLookup
                {
                    XPath = xPath,
                    Regex = regex
                };
                this.Add(dataMaskLookup);
            }
        }

        /// <summary>
        /// A look up for the data mask to use
        /// </summary>
        public class DataMaskLookup
        {
            /// <summary>
            /// Gets or sets the third party.
            /// </summary>
            /// <value>
            /// The third party.
            /// </value>
            public string ThirdParty { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action.
            /// </summary>
            /// <value>
            /// The action.
            /// </value>
            public string Action { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the x path.
            /// </summary>
            /// <value>
            /// The x path.
            /// </value>
            public string XPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the regex.
            /// </summary>
            /// <value>
            /// The regex.
            /// </value>
            public string Regex { get; set; } = string.Empty;
        }
    }
}
