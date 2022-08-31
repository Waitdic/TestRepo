namespace iVectorOne.Suppliers.DOTW
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Constants;
    using iVectorOne.DOTW;
    using iVectorOne.Lookups;

    public partial class DOTWSupport : IDOTWSupport
    {
        private readonly IDOTWSettings _settings;

        private readonly ITPSupport _support;

        private readonly HttpClient _httpClient;

        private readonly ILogger<DOTWSupport> _logger;

        private readonly IMemoryCache _cache;

        public DOTWSupport(
            IDOTWSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ILogger<DOTWSupport> logger,
            IMemoryCache cache)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _cache = Ensure.IsNotNull(cache, nameof(cache));
        }

        public partial class Cities : Dictionary<int, City>
        {
        }

        public partial class City
        {
            public int CityNumber;
            public List<int> LocationIDs = new();

            public City(int cityNumber, int locationId)
            {
                CityNumber = cityNumber;
                LocationIDs.Add(locationId);
            }
        }

        #region Password Hash

        public string MD5Password(string password)
        {
            var encoder = new UTF8Encoding();
            var md5Hasher = new MD5CryptoServiceProvider();
            var hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(password));

            string result = "";
            foreach (byte b in hashedDataBytes)
            {
                result += b.ToString("x2");
            }

            return result;
        }

        #endregion

        public int GetTitleID(string title)
        {
            Dictionary<string, int> cacheBuilder()
            {
                var titles = new Dictionary<string, int>();

                // read from res file
                using var titlesFile = new StringReader(DOTWRes.Titles);
                string titleLine = titlesFile.ReadLine();

                // loop through Titles,
                while (titleLine is not null)
                {
                    string key = titleLine.Split('#')[0];
                    int value = titleLine.Split('#')[1].ToSafeInt();

                    if (!titles.ContainsKey(key))
                    {
                        titles.Add(key, value);
                    }

                    titleLine = titlesFile.ReadLine();
                }
                titlesFile.Close();

                return titles;
            }

            var titles = _cache.GetOrCreate("DOTWTitle", cacheBuilder, 60);

            if (titles.ContainsKey(title))
            {
                return titles[title];
            }
            else
            {
                return titles["Mr"];
            }
        }

        public int GetCurrencyID(IThirdPartyAttributeSearch searchDetails)
        {
            // in ivector we are calling  SQL.GetValue("exec Geography_GetThirdPartyCurrencyCode). GeographyLevel1 is not configureable in iVectorOne
            // return default currencyID stored in the settings
            return _settings.DefaultCurrencyID(searchDetails);
        }

        public async Task<int> GetCurrencyCodeAsync(string isoCurrencyCode, IThirdPartyAttributeSearch searchDetails)
        {
            string currencyCode = await _support.TPCurrencyCodeLookupAsync(ThirdParties.DOTW, isoCurrencyCode);

            var currencyCache = await GetCurrencyCacheAsync(searchDetails);
            if (currencyCode != null && currencyCache.ContainsKey(currencyCode.ToSafeInt()))
            {
                return currencyCode.ToSafeInt();
            }
            else
            {
                return _settings.DefaultCurrencyID(searchDetails);
            }
        }

        public string CleanName(string name)
        {
            string cleanName = name;

            // Remove spaces
            cleanName = cleanName.Replace(" ", string.Empty);

            // Try and convert to latin characters. This also replaces special charaters and numbers
            cleanName = AngliciseString(cleanName);

            // If it's under 2 characters long then pad with X
            if (cleanName.Length < 2)
            {
                cleanName = cleanName.PadRight(2, 'X');
            }

            // Don't allow it to be longer than 25 characters
            if (cleanName.Length > 25)
            {
                cleanName = cleanName.Substring(0, 25);
            }

            return cleanName;
        }

        private string AngliciseString(string text)
        {
            return Regex.Replace(text.Normalize(NormalizationForm.FormD), "[^A-Za-z]*", string.Empty).Trim();
        }

        private async Task<Dictionary<int, string>> GetCurrencyCacheAsync(IThirdPartyAttributeSearch searchDetails)
        {
            async Task<Dictionary<int, string>> cacheBuilder()
            {
                var currencies = new Dictionary<int, string>();

                var sb = new StringBuilder();

                sb.AppendLine("<customer>");
                sb.AppendFormat("<username>{0}</username>", _settings.User(searchDetails)).AppendLine();
                sb.AppendFormat("<password>{0}</password>", MD5Password(_settings.Password(searchDetails))).AppendLine();
                sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(searchDetails)).AppendLine();
                sb.AppendLine("<source>1</source>");
                sb.AppendLine("<request command=\"getcurrenciesids\" />");
                sb.AppendLine("</customer>");

                // get the xml response for all currencies
                var headers = new RequestHeaders();
                if (_settings.UseGZip(searchDetails))
                {
                    headers.AddNew("Accept-Encoding", "gzip");
                }

                var webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.DOTW,
                    Headers = headers,
                    LogFileName = "GetCurrencyCache",
                    ContentType = ContentTypes.Text_xml,
                    CreateLog = true
                };
                webRequest.SetRequest(sb.ToString());
                await webRequest.Send(_httpClient, _logger);

                var responseXml = webRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                var successNode = responseXml.SelectSingleNode("result/successful");
                if (successNode is null || successNode.InnerText != "TRUE")
                {
                    throw new Exception("currencies do not return success");
                }

                foreach (XmlNode node in responseXml.SelectNodes("result/currency/option"))
                {
                    int key = node.SelectSingleNode("@value").Value.ToSafeInt();
                    string value = node.SelectSingleNode("@shortcut").Value; 

                    if (!currencies.ContainsKey(key))
                    {
                        currencies.Add(key, value);
                    }
                }

                return currencies;
            }

            return await _cache.GetOrCreateAsync("DOTWCurrency", cacheBuilder, 60);
        }
    }
}