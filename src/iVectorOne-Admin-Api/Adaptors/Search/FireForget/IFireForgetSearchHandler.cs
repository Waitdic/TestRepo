using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Adaptors.Search;

namespace iVectorOne_Admin_Api.Adaptors.Search.FireForget
{
    public interface IFireForgetSearchHandler
    {
        void Execute(Func<IFireForgetSearchOperation, Task> databaseWork);
    }
}
