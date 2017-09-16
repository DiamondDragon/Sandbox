using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SerioLogWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var hostingEnvironment = hostingContext.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true);

                    if (hostingEnvironment.IsDevelopment())
                    {
                        var assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
                        if (assembly != null)
                            config.AddUserSecrets(assembly, true);
                    }
                    config.AddEnvironmentVariables();
                    if (args == null)
                        return;

                    config.AddCommandLine(args);
                })
                //.ConfigureLogging((hostingContext, logging) =>
                //{
                //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                //    logging.AddConsole();
                //    logging.AddDebug();
                //})
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) => options.ValidateScopes = context.HostingEnvironment.IsDevelopment())
                .UseStartup<Startup>()
                .Build();
        }
    }
}
