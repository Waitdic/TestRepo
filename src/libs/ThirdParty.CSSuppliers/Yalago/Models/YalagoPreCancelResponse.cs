namespace ThirdParty.CSSuppliers.Models.Yalago
{
#pragma warning disable CS8618

    class YalagoPreCancelResponse
    {
        public bool IsCancellable { get; set; }
        public CancellationCharges charge { get; set; }
        public string ExpiryDate { get; set; }
        public string ExpiryDateUTC { get; set; }
        public CancellationPolicyStatic[] cancellationPolicyStatic { get; set; }

        public class Charge
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }

        public class CancellationPolicyStatic
        {
            public CancellationCharges[] cancellationCharges { get; set; }
            public string RoomName { get; set; }
        }

        public class CancellationCharges
        {
            public Charge charge { get; set; }
            public string ExpiryDate { get; set; }
            public string ExpiryDateUTC { get; set; }
        }
    }
}