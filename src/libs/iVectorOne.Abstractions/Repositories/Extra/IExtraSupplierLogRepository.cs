namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;
    using iVectorOne.Models.Extra;

    public interface IExtraSupplierLogRepository
    {
        Task LogPrebookRequestsAsync(ExtraDetails extraDetails);
        Task LogBookRequestsAsync(ExtraDetails extraDetails);
        Task LogPrecancelRequestsAsync(ExtraDetails extraDetails);
        Task LogCancelRequestsAsync(ExtraDetails extraDetails);
    }
}