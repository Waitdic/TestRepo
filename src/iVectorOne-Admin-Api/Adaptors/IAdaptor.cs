namespace iVectorOne_Admin_Api.Adaptors
{
    public interface IAdaptor<in TRequest, TResponse>
    //where TRequest : IRequest<TResponse>
    {

        Task<TResponse> Execute(TRequest request);
    }
}
