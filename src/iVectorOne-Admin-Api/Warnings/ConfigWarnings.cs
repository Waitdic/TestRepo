namespace iVectorOne_Admin_Api.Warnings
{
    public class ConfigWarnings
    {
        //Entity Warnings
        public const string NoUserWarning = "Could not find a user for the specified key";
        public const string NoTenantWarning = "Could not find a tenant for specified tenant id";
        public const string NoSubscriptionWarning = "Could not find a subscription with a matching id for the specified tenant";
        public const string NoSupplierWarning = "Could not find a supplier with a matching id for the specified subscription";
        public const string NoSuppliersWarning = "Could not find any suppliers";
        public const string MultiNoSupplierAttributesWarning = "Could not find any attributes for the specified subscription/supplier combination";
        public const string SingleNoSupplierAttributeWarning = "Could not find the specified supplier attribute";

        //Request Warnings
        public const string SubscriptionIDWarning = "SubscriptionID must be set and greater than 0";
        public const string SupplierIDWarning = "SupplierID must be set and greater than 0";
        public const string SupplierSubscriptionAttributeIDWarning = "SupplierSubscriptionAttributeID must be set and greater than 0";
        public const string TenantIDWarning = "TenantID must be set and greater than 0";
    }
}
