using System.Collections.Generic;
using Intuitive.Helpers.Net;

namespace iVectorOne.Models.SupplierLog
{
    public class SupplierLogs : List<SupplierBaseLog>
    {
        public void AddNew(string source, string title, Request request)
        {
            switch (title)
            {
                case "Pre-Book":
                case "Pre-Book Availability":
                    Add(new SupplierPrebookLog
                    {
                        SupplierName = source,
                        Request = request
                    });
                    break;
                case "Book":
                    Add(new SupplierBookLog
                    {
                        SupplierName = source,
                        Request = request
                    });
                    break;
            }
        }
    }
}
