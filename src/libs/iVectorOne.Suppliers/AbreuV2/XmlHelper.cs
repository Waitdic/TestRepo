namespace iVectorOne.Suppliers.AbreuV2
{
    using System.Xml;

    public static class XmlHelper
    {
        public static string CleanRequest(XmlDocument request)
        {
            var requestString = request.OuterXml;
            return requestString
                .Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "")
                .Replace(@"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "");
        }
    }
}
