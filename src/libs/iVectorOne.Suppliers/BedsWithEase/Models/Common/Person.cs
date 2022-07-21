namespace ThirdParty.CSSuppliers.BedsWithEase.Models.Common
{
    public class Person
    {
        public Age Age = new();

        public bool ShouldSerializeAge()
            => Age.Value != 0;
    }
}
