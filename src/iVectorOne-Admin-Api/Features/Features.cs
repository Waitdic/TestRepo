using iVectorOne_Admin_Api.Features.Errors;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Update;
using iVectorOne_Admin_Api.Features.V1.Tenants.Create;
using iVectorOne_Admin_Api.Features.V1.Tenants.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Disable;
using iVectorOne_Admin_Api.Features.V1.Tenants.Enable;
using iVectorOne_Admin_Api.Features.V1.Tenants.Info;
using iVectorOne_Admin_Api.Features.V1.Tenants.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Modify;
using iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link;
using iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink;
using iVectorOne_Admin_Api.Features.V1.Users.Create;
using iVectorOne_Admin_Api.Features.V1.Users.Info;
using iVectorOne_Admin_Api.Features.V1.Users.List;

namespace iVectorOne_Admin_Api.Features
{
    public static class Features
    {
        public static IApplicationBuilder AddFeatures(this WebApplication app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.MapErrorRoutes();

            //Tenant
            app.MapTenantsListV1Endpoint();
            app.MapTenantInfoV1Endpoint();
            app.MapTenantCreateV1Endpoint();
            app.MapTenantDeleteV1Endpoint();
            app.MapTenantDisableV1Endpoint();
            app.MapTenantEnableV1Endpoint();
            app.MapTenantUserLinkV1Endpoint();
            app.MapTenantUserUnlinkV1Endpoint();
            app.MapTenantModifyV1Endpoint();

            //Users
            app.MapUsersInfoV1Endpoint();
            app.MapUsersListV1Endpoint();
            app.MapUsersCreateV1Endpoint();

            //Account
            app.MapTenantAccountListV1Endpoint();
            app.MapTenantAccountInfoV1Endpoint();
            app.MapTenantAccountDeleteV1Endpoint();
            app.MapTenantAccountCreateV1Endpoint();
            app.MapTenantAccountUpdateV1Endpoint();

            //Supplier
            app.MapTenantAccountSupplierListV1Endpoint();
            app.MapTenantAccountSupplierDeleteV1Endpoint();
            app.MapTenantAccountSupplierTestV1Endpoint();

            return app;
        }
    }
}