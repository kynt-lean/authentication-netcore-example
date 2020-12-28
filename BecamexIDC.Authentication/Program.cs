using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BecamexIDC.Authentication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            //ChangePort(args);
        }
        /// <summary>
        /// Change Port
        /// But Problem with https
        /// If use https problem with connectionString
        /// </summary>
        static void ChangePort(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
            var Url = config.GetValue<string>("Host");// Set port 
            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, myconfig) =>
                    {
                        var env = hostingContext.HostingEnvironment;

                        myconfig.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                        if (env.IsDevelopment())
                        {
                            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                            if (appAssembly != null)
                            {
                                myconfig.AddUserSecrets(appAssembly, optional: true);
                            }
                        }

                        myconfig.AddEnvironmentVariables();

                        if (args != null)
                        {
                            myconfig.AddCommandLine(args);
                        }
                    })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                    })
               .UseUrls(Url)
                .Build();

            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders(); // removes all providers from LoggerFactory
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    logging.AddConsole(); //EnventSource, EventLog, TraceSource
                    logging.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                          
                    webBuilder.UseStartup<Startup>();
                });
    }
}
