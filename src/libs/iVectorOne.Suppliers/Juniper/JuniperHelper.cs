namespace iVectorOne.Suppliers.Juniper
{
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Suppliers.Juniper.Model;

    public static class JuniperHelper
    {
        #region Request Builder

        public static Request BuildWebRequest(
            string url,
            string soapAction,
            string request,
            string logFileName,
            string source,
            bool useGZip = false)
        {
            var webRequest = new Request
            {
                EndPoint = url,
                SOAP = true,
                SoapAction = soapAction,
                Method = RequestMethod.POST,
                Source = source,
                LogFileName = logFileName,
                UseGZip = useGZip,
            };

            webRequest.SetRequest(request);

            return webRequest;
        }

        internal static Pos BuildPosNode(string agentDutyCode, string password)
        {
            return new Pos
            {
                Source = {
                    AgentDutyCode = agentDutyCode,
                    RequestorId =
                    {
                        TypeCode = Constant.RequestorTypeCode,
                        MessagePassword = password
                    }
                }
            };
        }

        #endregion

        #region Common methods

        public static string BuildSoap<T>(T request, ISerializer serializer)
        {
            string elementName = "";
            var requestStr = serializer.Serialize(request).OuterXml;
            var bodyContentClass = request.GetType().Name;
            var bodyContentElementName = !string.IsNullOrEmpty(elementName) ? elementName : bodyContentClass;
            if (!string.Equals(bodyContentClass, bodyContentElementName))
            {
                requestStr = requestStr.Replace(bodyContentClass, bodyContentElementName);
            }
            requestStr = requestStr.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");

            requestStr = requestStr.Replace($"<{bodyContentElementName}>",
                                            $"<{bodyContentElementName} xmlns=\"http://www.opentravel.org/OTA/2003/05\">");

            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
                    + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
                    + $" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""
                    + $" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n"
                    + $"<soap:Body>\n{requestStr}\n</soap:Body>\n"
                    + $"</soap:Envelope>";
        }

        public static string ConstructUrl(string baseUrl, string urlPath)
        {
            if (baseUrl.EndsWith("/")) baseUrl.Substring(0, baseUrl.Length - 1);
            if (urlPath.StartsWith("/")) urlPath.Substring(1, urlPath.Length - 1);
            return $"{baseUrl}/{urlPath}";
        }

        #endregion
    }
}