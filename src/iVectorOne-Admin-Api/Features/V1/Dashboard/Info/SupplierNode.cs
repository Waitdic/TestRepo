using Microsoft.Identity.Client;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public class SupplierNode
    {
        public void NodeSet(string Name)
        {
            Random random = new Random();
            var name = Name;
            var searchTotal = random.Next(6000);
            var searchSuccess = random.Next(80,100);
            var avgResponse = random.Next(1500,3000);
            var prebookTotal = random.Next(300);
            var prebookSuccess = random.Next(85,100);
            var bookTotal = random.Next(20);
            var bookSuccess = random.Next(95,100) ;
            var bookS2B = random.Next(500,1000);
        }
    }
}