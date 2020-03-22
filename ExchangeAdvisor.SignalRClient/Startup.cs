using ExchangeAdvisor.DB.Migrations;
using ExchangeAdvisor.DB.Repositories;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.ML;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.EJ2.Blazor;
using Syncfusion.Licensing;

namespace ExchangeAdvisor.SignalRClient
{
    public class Startup
    {
        public Startup(IConfiguration applicationConfiguration)
        {
            configurationReader = new ApplicationConfigurationReader(applicationConfiguration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();

            services.AddHttpClient();
            services.AddSingleton<IConfigurationReader>(configurationReader);
            services.AddScoped<IRateWebFetcher, RateWebFetcher>();
            services.AddScoped<IRateForecaster, RateForecaster>();
            services.AddScoped<IHistoricalRatesRepository, HistoricalRatesRepository>();
            services.AddScoped<IRateService, RateService>();
        }

        public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment environment)
        {
            SyncfusionLicenseProvider.RegisterLicense(configurationReader.SyncfusionLicenseKey);

            new DatabaseMigrator(configurationReader.DatabaseConnectionString).Migrate();

            if (environment.IsDevelopment())
            {
                appBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                appBuilder.UseExceptionHandler("/Error");
                appBuilder.UseHsts();
            }

            appBuilder.UseHttpsRedirection();
            appBuilder.UseStaticFiles();

            appBuilder.UseRouting();

            appBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private readonly ApplicationConfigurationReader configurationReader;
    }
}
