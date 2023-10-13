using ChangeTokenTest;

namespace ChangeTokenExample
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            // change token
            configuration.OnChange(() => InvokeChanged(loggerFactory));

            // let request routes to endpoint
            app.UseRouting();

            // endpoint maps to controller
            app.UseEndpoints(endpoint => endpoint.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // add all controllers
            services.AddControllers();

            services.AddMemoryCache();
            services.AddSingleton<FileCache>();
        }

        private void InvokeChanged(ILoggerFactory loggerFactory)
        {
            loggerFactory.CreateLogger<Startup>().LogWarning($"{DateTime.Now} Configuration changed");
        }
    }
}
