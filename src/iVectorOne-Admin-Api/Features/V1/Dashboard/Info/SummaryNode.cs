using Microsoft.Identity.Client;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public class SummaryNode
    {
        public void NodeSet(string day)
        {
            Random random = new Random();
            var name = day;
            var bookingsTotal = random.Next(1000);
            var bookingsValue = random.Next(500000);
            var prebookTotal = random.Next(40000);
            var prebookSuccess = random.Next(65, 95);
            var searchTotal = random.Next(700000);
            var searchSuccessful = random.Next(65, 95);
            var avgResponse = random.Next(2000, 3000);
            var S2B = random.Next(500,1000);
        }
    }
}