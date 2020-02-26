using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ExchangeAdvisor.SignalRClient
{
    [SuppressMessage("Design", "RCS1102:Make class static.", Justification = "Framework requirement")]
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Framework requirement")]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}
