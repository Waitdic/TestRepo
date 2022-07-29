namespace iVectorOne.Suppliers.BedsWithEase
{
    using Intuitive.Helpers.Net;
    using System.Xml;

    public class Result
    {
        public Result(Request request, XmlDocument response)
        {
            Request = request;
            Response = response;
        }

        public Request Request { get; } = new();
        public XmlDocument Response { get; } = new();
    }
}