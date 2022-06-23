namespace iVectorOne_Admin_Api.Services
{
    public static class AppExtensions
    {
        public static WebApplication ConfigureApp(this WebApplication app)
        {
            app.UseHttpsRedirection();
            var allowedHosts = app.Configuration.GetSection("CorsOrigins").Get<string[]>();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins(allowedHosts));

            app.UsePathBase(new PathString("/admin/v1"));
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static WebApplication ConfigureSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }
    }
}
