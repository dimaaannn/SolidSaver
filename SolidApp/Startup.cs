using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidApp.SW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;

using NLog.Extensions.Logging;


namespace SolidApp
{
    public static class Startup
    {


        public static IServiceProvider ServiceProvider { get; private set; }
        public static void Init(string[] args)
        {
            //var logger = new NLog.LogFactory().Setup()
            //    .LoadConfigurationFromFile()
            //    .GetCurrentClassLogger();

            //var logger2 = LogManager.Setup()
            //    .LoadConfigurationFromFile();

            var host = Host.CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config
                //        .AddJsonFile()
                //})
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddLogging();
                    
                    services.AddSingleton<ISwConnector, SWConnector>();
                    services.AddTransient<NotifyOnInstance>();
                })
                .ConfigureLogging(logging =>
                {
                    //logging.AddConsole()
                    logging.AddNLog("NLog.config")
                        .SetMinimumLevel(LogLevel.Trace);
                })
                .Build();


            ServiceProvider = host.Services;
            
        }
    }
}
