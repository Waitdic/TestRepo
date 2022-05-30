namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses
{

    public interface IExpediaRapidResponse
    {

        bool IsValid(string responseString, int statusCode);

    }

}