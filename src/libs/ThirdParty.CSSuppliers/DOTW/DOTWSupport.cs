namespace ThirdParty.CSSuppliers.DOTW
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public partial class DOTWSupport : IDOTWSupport
    {

        public partial class Cities : Dictionary<int, City>
        {

        }

        public partial class City
        {

            public int CityNumber;
            public List<int> LocationIDs = new List<int>();

            public City(int CityNumber, int LocationID)
            {
                this.CityNumber = CityNumber;
                LocationIDs.Add(LocationID);
            }

        }

        public static string StripNameSpaces(string sXML)
        {

            sXML = sXML.Replace("xmlns=\"http://xmldev.dotwconnect.com/xsd/\"", "");
            sXML = sXML.Replace("xmlns:a=\"http://xmldev.dotwconnect.com/xsd/atomicCondition_rooms\"", "");
            sXML = sXML.Replace("xmlns:c=\"http://xmldev.dotwconnect.com/xsd/complexCondition_rooms\"", "");

            return sXML;

        }


        #region Password Hash

        public static string MD5Password(string sPassword)
        {

            var encoder = new UTF8Encoding();
            /* TODO ERROR: Skipped WarningDirectiveTrivia
            #Disable Warning SCS0006 ' Weak hashing function
            */
            var md5Hasher = new MD5CryptoServiceProvider();
            /* TODO ERROR: Skipped WarningDirectiveTrivia
            #Enable Warning SCS0006 ' Weak hashing function
            */
            var hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(sPassword));

            string sResult = "";
            foreach (byte b in hashedDataBytes)
                sResult += b.ToString("x2");

            return sResult;

        }

        #endregion

        public static int GetTitleID(string sTitle)
        {

            // HACK CS Dim oTitle As Generic.Dictionary(Of String, Integer) = CType(HttpRuntime.Cache("DOTWTitle"), Generic.Dictionary(Of String, Integer))

            Dictionary<string, int> oTitle = null;

            if (oTitle is null)
            {

                oTitle = new Dictionary<string, int>();

                // read from res file
                var oTitles = new StringReader(DOTWRes.Titles);
                string sTitlesLine = oTitles.ReadLine();


                // loop through Titles, 
                while (sTitlesLine is not null)
                {

                    string sKey = sTitlesLine.Split('#')[0];
                    int iValue = sTitlesLine.Split('#')[1].ToSafeInt();

                    if (!oTitle.ContainsKey(sKey))
                    {
                        oTitle.Add(sKey, iValue);
                    }
                    sTitlesLine = oTitles.ReadLine();
                }
                oTitles.Close();

                // HACK CS Intuitive.Functions.AddToCache("DOTWTitle", oTitle, 60)

            }


            if (oTitle.ContainsKey(sTitle))
            {
                return oTitle[sTitle];
            }
            else
            {
                return oTitle["Mr"];
            }


        }

        public static DOTWCurrencyCache GetCurrencyCache(IThirdPartyAttributeSearch SearchDetails, IDOTWSettings Settings, HttpClient client, ILogger<DOTW> logger) 
        {

            // HACK CS Dim oCurrency As DOTWCurrencyCache = CType(HttpRuntime.Cache("DOTWCurrency"), DOTWCurrencyCache)

            DOTWCurrencyCache oCurrency = null;

            if (oCurrency is null)
            {


                oCurrency = new DOTWCurrencyCache();

                var oSB = new StringBuilder();

                oSB.AppendLine("<customer>");
                oSB.AppendFormat("<username>{0}</username>", Settings.Username(SearchDetails)).AppendLine();
                oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(Settings.Password(SearchDetails))).AppendLine();
                oSB.AppendFormat("<id>{0}</id>", Settings.CompanyCode(SearchDetails)).AppendLine();
                oSB.AppendLine("<source>1</source>");
                oSB.AppendLine("<request command=\"getcurrenciesids\" />");
                oSB.AppendLine("</customer>");

                // get the xml response for all currencies
                var oResponseXML = new XmlDocument();

                var oWebRequest = new Intuitive.Net.WebRequests.Request();

                var oHeaders = new Intuitive.Net.WebRequests.RequestHeaders();
                if (Settings.UseGZip(SearchDetails))
                {
                    oHeaders.AddNew("Accept-Encoding", "gzip");
                }

                oWebRequest.EndPoint = Settings.ServerURL(SearchDetails);
                oWebRequest.Method = eRequestMethod.POST;
                oWebRequest.Source = ThirdParties.DOTW;
                oWebRequest.Headers = oHeaders;
                oWebRequest.LogFileName = "GetCurrencyCache";
                oWebRequest.SetRequest(oSB.ToString());
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.CreateLog = true;
                oWebRequest.Send(client, logger);

                oResponseXML = oWebRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                var oSuccessNode = oResponseXML.SelectSingleNode("result/successful");
                if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
                {
                    throw new Exception("currencies do not return success");
                }


                foreach (XmlNode oNode in oResponseXML.SelectNodes("result/currency/option"))
                {
                    string sKey = oNode.SelectSingleNode("@shortcut").Value;
                    int iValue = oNode.SelectSingleNode("@value").Value.ToSafeInt();

                    if (!oCurrency.ContainsKey(sKey))
                    {
                        oCurrency.Add(sKey, iValue);
                    }

                }

                // HACK CS Intuitive.Functions.AddToCache("DOTWCurrency", oCurrency, 60)

            }

            return oCurrency;

        }

        public int GetCachedCurrencyID(IThirdPartyAttributeSearch SearchDetails, ITPSupport Support, string CurrencyCode, IDOTWSettings Settings)
        {
            return GetCurrencyID(SearchDetails, Support, CurrencyCode, Settings);
        }

        public static int GetCurrencyID(IThirdPartyAttributeSearch SearchDetails, ITPSupport Support, string CurrencyCode, IDOTWSettings Settings)
        {
            // in ivector we are calling  SQL.GetValue("exec Geography_GetThirdPartyCurrencyCode). GeographyLevel1 is not configureable in iVectorOne
            // return default currencyID stored in the settings
            return Settings.DefaultCurrencyID(SearchDetails);
        }

        public static int GetCurrencyCode(int currencyID, IThirdPartyAttributeSearch searchDetails, IDOTWSettings settings, ITPSupport support, HttpClient client, ILogger<DOTW> logger)
        {
            string sCurrencyCode = support.TPCurrencyLookup(ThirdParties.DOTW, currencyID.ToSafeString());

            var oCurrencyCache = GetCurrencyCache(searchDetails, settings, client, logger);
            if (oCurrencyCache.ContainsKey(sCurrencyCode))
            {
                return oCurrencyCache[sCurrencyCode];
            }
            else
            {
                return settings.DefaultCurrencyID(searchDetails);
            }

        }

        internal static string CleanName(string name, ITPSupport support)
        {

            string sCleanName = name;

            // Remove spaces
            sCleanName = sCleanName.Replace( " ", string.Empty);

            // Try and convert to latin characters. This also replaces special charaters and numbers
            sCleanName = AngliciseString(sCleanName);

            // If it's under 2 characters long then pad with X
            if (sCleanName.Length < 2)
            {
                sCleanName = sCleanName.PadRight(2, 'X');
            }

            // Don't allow it to be longer than 25 characters
            if (sCleanName.Length > 25)
            {
                sCleanName = sCleanName.Substring(0, 25);
            }

            return sCleanName;

        }

        public static string AngliciseString(string text)
        {
            return Regex.Replace(text.Normalize(NormalizationForm.FormD), "[^A-Za-z]*", string.Empty).Trim();
        }
    }

    public class DOTWCurrencyCache : Dictionary<string, int> { }
}
