using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidApp.SW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidApp
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static void Init(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<ISwConnector, SWConnector>();
                    services.AddTransient<NotifyOnInstance>();
                }).Build();
            ServiceProvider = host.Services;
            
        }
    }
}
