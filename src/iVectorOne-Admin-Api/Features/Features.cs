using iVectorOne_Admin_Api.Features.Errors;
using iVectorOne_Admin_Api.Features.V1.Tenants.Create;
using iVectorOne_Admin_Api.Features.V1.Tenants.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Disable;
using iVectorOne_Admin_Api.Features.V1.Tenants.Enable;
using iVectorOne_Admin_Api.Features.V1.Tenants.Info;
using iVectorOne_Admin_Api.Features.V1.Tenants.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Modify;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Create;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Info;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Update;
using iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link;
using iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink;
using iVectorOne_Admin_Api.Features.V1.Users.Info;

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

            app.MapUsersListV1Endpoint();
            app.MapUsersTenantsListV1Endpoint();

            //Tenant
            app.MapTenantInfoV1Endpoint();
            app.MapTenantCreateV1Endpoint();
            app.MapTenantDeleteV1Endpoint();
            app.MapTenantDisableV1Endpoint();
            app.MapTenantEnableV1Endpoint();
            app.MapTenantUserLinkV1Endpoint();
            app.MapTenantUserUnlinkV1Endpoint();
            app.MapTenantModifyV1Endpoint();

            //Subscription
            app.MapTenantSubscriptionsListV1Endpoint();
            app.MapTenantSubscriptionInfoV1Endpoint();
            app.MapTenantSubscriptionDeleteV1Endpoint();
            app.MapTenantSubscriptionCreateV1Endpoint();
            app.MapTenantSubscriptionUpdateV1Endpoint();

            //Supplier
            app.MapTenantSubscriptionSupplierListV1Endpoint();
            app.MapTenantSubscriptionSupplierDeleteV1Endpoint();

            return app;
        }
    }
}
