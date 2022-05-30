namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Cancel
{

    public class CancelResponse : IExpediaRapidResponse
    {

        public bool IsValid(string responseString, int statusCode)
        {

            switch (statusCode)
            {
                case 202:
                    {
                        return true;
                    }
                case 204:
                    {
                        return true;
                    }
            }

            return false;
        }
    }

}