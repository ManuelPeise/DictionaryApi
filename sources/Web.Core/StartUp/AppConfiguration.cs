namespace Web.Core.StartUp
{
    internal static class AppConfiguration
    {
        internal static void ConfigureApplication(this WebApplication app, string corsPolicy)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(corsPolicy);

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
