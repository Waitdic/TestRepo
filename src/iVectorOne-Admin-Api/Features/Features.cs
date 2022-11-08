using iVectorOne_Admin_Api.Features.Errors;
using iVectorOne_Admin_Api.Features.V1.Dashboard.Info;
using iVectorOne_Admin_Api.Features.V1.Properties.Search;
using iVectorOne_Admin_Api.Features.V1.Suppliers.Info;
using iVectorOne_Admin_Api.Features.V1.Suppliers.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.List;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Delete;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Info;
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
using iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer;
using iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer;
using iVectorOne_Admin_Api.Features.V1.Utilities.LogViewerDetail;
using iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest;
using iVectorOne_Admin_Api.Features.V2.Suppliers.Info;
using iVectorOne_Admin_Api.Features.V2.Tenants.Accounts.Suppliers.Info;

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

            app.MapTenantAccountSupplierInfoV1Endpoint();

            app.MapTenantAccountSupplierInfoV2Endpoint();
            app.MapSupplierInfoV2Endpoint();
            app.MapSupplierListV1Endpoint();
            app.MapSupplierInfoV1Endpoint();

            //Utilities
            app.MapSearchTestV1Endpoint();
            app.MapLogViewerV1Endpoint();
            app.MapBookingViewerV1Endpoint();
            app.MapPropertiesSearchV1Endpoint();
            app.MapLogViewerDetailV1Endpoint();

            //Dashboard
            app.MapDashboardInfoV1Endpoint();

            return app;
        }
    }
}