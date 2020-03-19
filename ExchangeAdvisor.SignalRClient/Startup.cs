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
        public Startup(IConfiguration configuration)
        {
            configurationReader = new ConfigurationReader(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();

            services.AddHttpClient();
            services.AddScoped<IRateWebFetcher, RateWebFetcher>();
            services.AddScoped<IRateForecaster, RateForecaster>();
            services.AddScoped<IHistoricalRatesRepository, HistoricalRatesRepository>(
                _ => new HistoricalRatesRepository(configurationReader.DatabaseConnectionString));
            services.AddScoped<IRateService, RateService>();
        }

        public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment environment)
        {
            SyncfusionLicenseProvider.RegisterLicense(configurationReader.SyncfusionLicenseKey);

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

        private readonly ConfigurationReader configurationReader;
    }
}
