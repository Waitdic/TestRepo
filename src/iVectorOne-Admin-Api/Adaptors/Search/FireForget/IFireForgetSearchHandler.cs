namespace iVectorOne_Admin_Api.Adaptors.Search.FireForget
{
    public interface IFireForgetSearchHandler
    {
        void Execute(Func<IAdaptor<Request, Response>, Task> databaseWork);
    }
}
