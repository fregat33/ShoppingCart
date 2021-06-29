using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ShoppingCart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetCurrentDirectoryToAppLocation();
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(
                    (hostingContext, loggerConfiguration) => loggerConfiguration
                                                             .ReadFrom.Configuration(hostingContext.Configuration)
                                                             .Enrich.FromLogContext()
                )
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static void SetCurrentDirectoryToAppLocation()
        {
            var pathToApp = GetPathToApp();
            var directoryName = Path.GetDirectoryName(pathToApp);
            Directory.SetCurrentDirectory(directoryName);
        }

        private static string GetPathToApp()
        {
            var path = Process.GetCurrentProcess()?.MainModule?.FileName;
            if (path == null || Path.GetFileNameWithoutExtension(path).Equals("dotnet"))
                return Assembly.GetExecutingAssembly().Location;

            return path;
        }
    }
}