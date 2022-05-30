using System.Xml;

namespace ThirdParty.CSSuppliers.BedsWithEase
{
    public class Result
    {
        public Result(XmlDocument request, XmlDocument response)
        {
            Request = request;
            Response = response;
        }

        public XmlDocument? Request { get; }
        public XmlDocument? Response { get; }
    }
}