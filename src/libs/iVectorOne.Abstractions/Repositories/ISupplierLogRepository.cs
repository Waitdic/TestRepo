using System;
using System.Threading.Tasks;
using iVectorOne.Models;
using iVectorOne.Models.Property.Booking;
using iVectorOne.Models.SupplierLog;
using iVectorOne.SDK.V2.PropertyBook;

namespace iVectorOne.Repositories
{
    public interface ISupplierLogRepository
    {
        Task LogPrebookAsync(SupplierLogs supplierLogs, Account account, DateTime prebookDateAndTime);

        Task LogBookAsync(SupplierLogs supplierLogs, Account account, DateTime bookDateAndTime,
            PropertyDetails propertyDetails, Response response, bool success);
    }
}
