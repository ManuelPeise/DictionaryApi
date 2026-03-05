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
            //app.UseMiddleware<SchedulerMiddleWare>();

            // Only use HTTPS redirection when not running in Docker
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
            {
                // Running in Docker - skip HTTPS redirection
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
