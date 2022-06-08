namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Cancel
{
    public class CancelResponse : IExpediaRapidResponse<CancelResponse>
    {
        public (bool valid, CancelResponse response) GetValidResults(string responseString, int statusCode)
        {
            (bool valid, CancelResponse response) = (false, new CancelResponse());
            switch (statusCode)
            {
                case 202:
                    {
                        valid = true;
                        break;
                    }
                case 204:
                    {
                        valid = true;
                        break;
                    }
            }

            return (valid, response);
        }
    }
}