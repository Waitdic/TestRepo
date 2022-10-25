using iVectorOne_Admin_Api.Features;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record ResponseModel : ResponseModelBase
    {
        public List<Node> BookingsByHour { get; set; }  =new List<Node>();
        public List<Node> SearchesByHour { get; set; } = new List<Node>();
        public List<Summary> Summary { get; set; } = new List<Summary>();
        public List<Supplier> Supplier { get; set;} = new List<Supplier>();

        //public Response Account { get; set; } = new Response();

        //public List<Node> Suppliers()
        //{
        //    List<Node> nodes = new List<Node>();
        //        nodes.Add(new Node());
        //        nodes[0].NodeSetSearch("today");
        //        nodes.Add(new Node());
        //        nodes[1].NodeSetSearch("WTD");
        //        nodes.Add(new Node());
        //        nodes[2].NodeSetSearch("MTD");
        //    return nodes;
        //}
        //public List<Node> Summary()
        //{
        //    List<Node> nodes = new List<Node>();
        //        nodes.Add(new Node());
        //        nodes[0].NodeSetSearch("HotelBeds");
        //        nodes.Add(new Node());
        //        nodes[1].NodeSetSearch("SunHotels");
        //        nodes.Add(new Node());
        //        nodes[2].NodeSetSearch("Miki");
        //        nodes.Add(new Node());
        //        nodes[3].NodeSetSearch("Stuba");
        //    return nodes;
        //}
    }
}