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
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();

            services.AddHttpClient();
            services.AddScoped<IRateWebFetcher, RateWebFetcher>();
            services.AddScoped<IRateForecaster, RateForecaster>();
            services.AddScoped<IHistoricalRatesRepository, HistoricalRatesRepository>();
            services.AddScoped<IRateService, RateService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            SyncfusionLicenseProvider.RegisterLicense("MjI0NjcwQDMxMzcyZTM0MmUzMG51eUxNa3JXYWJHb0dkYXVvU3pXNTRVbENkNGVBc2FGNERDa1Y4LzkyZ0E9"); // TODO: provide key with configuration

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
