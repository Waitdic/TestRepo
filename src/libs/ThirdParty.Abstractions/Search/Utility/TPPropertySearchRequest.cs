//namespace ThirdParty.Search.Utility
//{
//    using System.Text;

//    public class TPPropertySearchRequest
//    {
//        #region "Properties"

//        public StringBuilder sbRequestBuilder = new StringBuilder();
//        private string sRequestName = "";
//        private string sCustomNameSpace = "";
//        private bool bSoapRequest = false;

//        #endregion

//        #region "Constructors"

//        public TPPropertySearchRequest()
//        {
//        }

//        #endregion

//        public void AppendOpenRequestString(string RequestName)
//        {
//            sRequestName = RequestName;
//            sbRequestBuilder.AppendFormat("{0}" + sRequestName + "{1}", "<", ">");
//        }

//        public void AppendOpenSOAPRequestString(string RequestName, string CustomOpenSOAPText = "", string CustomNameSpace = "")
//        {
//            bSoapRequest = true;
//            sbRequestBuilder.Append(CustomOpenSOAPText);
//            sCustomNameSpace = CustomNameSpace;
//            AppendOpenRequestString(RequestName);
//        }

//        public void AppendCloseRequestString()
//        {
//            sbRequestBuilder.AppendFormat("{0}" + sRequestName + "{1}", "</", ">");
//            if (bSoapRequest)
//            {
//                if (sCustomNameSpace == string.Empty)
//                {
//                    sbRequestBuilder.Append("</soap:Body>");
//                    sbRequestBuilder.Append("</soap:Envelope>");
//                }
//                else
//                {
//                    sbRequestBuilder.AppendFormat("</{0}:Body>", sCustomNameSpace);
//                    sbRequestBuilder.AppendFormat("</{0}:Envelope>", sCustomNameSpace);
//                }
//            }
//        }

//        public Intuitive.Net.WebRequests.Request Create(string URL, string Source, bool SaveLogs, int TimeOut, object ExtraInfo, Intuitive.Net.WebRequests.CallbackDelegate ResponseCallBack, string SOAPAction = "")
//        {
//            Intuitive.Net.WebRequests.Request oWebRequest = new Intuitive.Net.WebRequests.Request();
//            {
//                var withBlock = oWebRequest;
//                withBlock.EndPoint = URL;
//                withBlock.SetRequest(sbRequestBuilder.ToString());
//                withBlock.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
//                withBlock.Source = Source;
//                withBlock.LogFileName = "Search";
//                withBlock.TimeoutInSeconds = TimeOut;
//                withBlock.CreateLog = SaveLogs;
//                withBlock.ExtraInfo = ExtraInfo;
//                withBlock.ReturnFunction = ResponseCallBack;
//                withBlock.UseGZip = true;
//                if (SOAPAction != "")
//                    withBlock.SoapAction = SOAPAction;
//            }
//            return oWebRequest;
//        }
//    }
//}
