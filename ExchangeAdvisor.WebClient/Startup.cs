using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Services.Implementation;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeAdvisor.WebClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IExchangeRateFetcher, ExchangeRateFetcher>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
