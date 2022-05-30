//namespace ThirdParty.Support
//{
//    using System;
//    using System.Text;
//    using System.Xml;
//    using Intuitive;
//    using Intuitive.Helpers.Extensions;

//    /// <summary>
//    /// A class containing static logging support functions, copied from part of third party support in third party interfaces
//    /// </summary>
//    public class LoggingSupport
//    {
//        /// <summary>
//        /// Adds the search log entry.
//        /// </summary>
//        /// <param name="thirdPartyName">Name of the third party.</param>
//        /// <param name="salesChannelID">The sales channel identifier.</param>
//        /// <param name="brandID">The brand identifier.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="text">The text.</param>
//        /// <param name="email">if set to <c>true</c> [email].</param>
//        /// <param name="emailTo">The email to.</param>
//        /// <param name="smtpHost">The SMTP host.</param>
//        /// <param name="xMLDoc">The x ml document.</param>
//        /// <param name="dataMasker">The data masker.</param>
//        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
//        public static void AddSearchLogEntry(string thirdPartyName, int salesChannelID, int brandID, string title, string text, bool email = false, string emailTo = "", string smtpHost = "", XmlDocument xMLDoc = null!, DataMasking.DataMaskLookups dataMasker = null!, bool isDebug = false)
//        {
//            LoggingSupport.LogEntry(true, thirdPartyName, salesChannelID, brandID, title, text, email, emailTo, smtpHost, xMLDoc, dataMasker, isDebug);
//        }

//        /// <summary>
//        /// Adds the search log entry.
//        /// </summary>
//        /// <param name="thirdPartyName">Name of the third party.</param>
//        /// <param name="searchDetails">The search details.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="text">The text.</param>
//        /// <param name="email">if set to <c>true</c> [email].</param>
//        /// <param name="emailTo">The email to.</param>
//        /// <param name="smtpHost">The SMTP host.</param>
//        /// <param name="xMLDoc">The x ml document.</param>
//        /// <param name="dataMasker">The data masker.</param>
//        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
//        public static void AddSearchLogEntry(
//            string thirdPartyName,
//            IThirdPartyAttributeSearch searchDetails,
//            string title,
//            string text,
//            bool email = false,
//            string emailTo = "",
//            string smtpHost = "",
//            XmlDocument xMLDoc = null!,
//            DataMasking.DataMaskLookups dataMasker = null!,
//            bool isDebug = false)
//        {
//            LoggingSupport.LogEntry(true, thirdPartyName, searchDetails.SalesChannelID, searchDetails.BrandID, title, text, email, emailTo, smtpHost, xMLDoc, dataMasker, isDebug);
//        }

//        /// <summary>
//        /// Adds the log entry.
//        /// </summary>
//        /// <param name="thirdPartyName">Name of the third party.</param>
//        /// <param name="salesChannelID">The sales channel identifier.</param>
//        /// <param name="brandID">The brand identifier.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="text">The text.</param>
//        /// <param name="email">if set to <c>true</c> [email].</param>
//        /// <param name="emailTo">The email to.</param>
//        /// <param name="smtpHost">The SMTP host.</param>
//        /// <param name="xMLDoc">The x ml document.</param>
//        /// <param name="dataMasker">The data masker.</param>
//        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
//        /// <param name="loggingType">Type of the logging.</param>
//        public static void AddLogEntry(string thirdPartyName, int salesChannelID, int brandID, string title, string text, bool email = false, string emailTo = "", string smtpHost = "", XmlDocument xMLDoc = null!, DataMasking.DataMaskLookups dataMasker = null!, bool isDebug = false, string loggingType = "None")
//        {
//            LoggingSupport.LogEntry(false, thirdPartyName, salesChannelID, brandID, title, text, email, emailTo, smtpHost, xMLDoc, dataMasker, isDebug);
//        }

//        /// <summary>Emails the search logs.</summary>
//        /// <param name="emailAddress">The email address.</param>
//        /// <param name="requestedProvider">The requested provider.</param>
//        /// <param name="provider">The provider.</param>
//        /// <param name="requestXML">The request XML.</param>
//        /// <param name="responseXML">The response XML.</param>
//        /// <param name="smtpHost">The host for sending the emails</param>
//        public static void EmailSearchLogs(string emailAddress, string requestedProvider, string provider, string requestXML, string responseXML, string smtpHost)
//        {
//            try
//            {
//                // build and send email
//                if (!string.IsNullOrEmpty(emailAddress) && requestedProvider.ToLower().InList(string.Empty, provider.ToLower()))
//                {
//                    var sb = new StringBuilder();
//                    sb.Append("Request").AppendLine().AppendLine();
//                    sb.Append(requestXML).AppendLine().AppendLine();
//                    sb.Append("Response").AppendLine().AppendLine();
//                    sb.Append(responseXML);

//                    var email = new Email()
//                    {
//                        SMTPHost = smtpHost,
//                        EmailTo = emailAddress,
//                        From = string.Format("{0} Search Logs", provider),
//                        FromEmail = "searchlogs@intuitivesystems.co.uk",
//                        Subject = string.Format("Search Logs - {0} {1}", provider, DateTime.Now),
//                        Body = sb.ToString()
//                    };

//                    email.SendEmail();
//                }
//            }
//            catch
//            {
//            }
//        }

//        /// <summary>
//        /// Logs the entry.
//        /// </summary>
//        /// <param name="isSearchLog">if set to <c>true</c> [is search log].</param>
//        /// <param name="thirdPartyName">Name of the third party.</param>
//        /// <param name="salesChannelID">The sales channel identifier.</param>
//        /// <param name="brandID">The brand identifier.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="text">The text.</param>
//        /// <param name="email">if set to <c>true</c> [email].</param>
//        /// <param name="emailTo">The email to.</param>
//        /// <param name="smtpHost">The SMTP host.</param>
//        /// <param name="xMLDoc">The x ml document.</param>
//        /// <param name="dataMasker">The data masker.</param>
//        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
//        /// <param name="loggingType">Type of the logging.</param>
//        private static void LogEntry(bool isSearchLog, string thirdPartyName, int salesChannelID, int brandID, string title, string text, bool email = false, string emailTo = "", string smtpHost = "", XmlDocument xMLDoc = null!, DataMasking.DataMaskLookups dataMasker = null!, bool isDebug = false, string loggingType = "None")
//        {
//            if (loggingType == "None" & !isDebug)
//            {
//                return;
//            }

//            // remove various bits from the logging
//            xMLDoc = DataMasking.DocumentReplace(xMLDoc, dataMasker);
//            if (isDebug)
//            {
//                string module = thirdPartyName.Replace(" ", string.Empty);
//                Intuitive.FileFunctions.AddLogEntry(module, title, text, email, emailTo, smtpHost, xMLDoc);
//            }
//            else
//            {
//                // trim down the error if it's a TP Search failure and we don't have full logging turned on
//                if (loggingType == "ExcludeSearch" && (title == "Search Failure" || title == "SearchError") && text.Length > 5000)
//                {
//                    text = text.Substring(0, 5000);
//                }

//                if (loggingType == "All" || (loggingType == "ExcludeSearch" && !isSearchLog))
//                {
//                    string module = thirdPartyName.Replace(" ", string.Empty);
//                    Intuitive.FileFunctions.AddLogEntry(module, title, text, email, emailTo, smtpHost, xMLDoc);
//                }
//            }
//        }
//    }
//}
