namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses
{
    public interface IExpediaRapidResponse<TResponse>
    {
        (bool valid, TResponse response) GetValidResults(string responseString, int statusCode);
    }
}