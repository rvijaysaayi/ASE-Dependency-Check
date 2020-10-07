using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASEDependencyCheck.Controller;
using ASEDependencyCheck.Services;
using ASEDependencyCheck.Services.Interfaces;
using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ase_dependency_check
{
    public class Program
    {

        public static int Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ISocketConnection, SocketConnection>();
            services.AddSingleton<ConnectivityTestController>();



            var serilogLogger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .WriteTo.RollingFile("ase-dependency-test-results.log")
                                .CreateLogger();

            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddSerilog(logger: serilogLogger, dispose: true);

            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            AppRunner<ConnectivityTestController> appRunner = new AppRunner<ConnectivityTestController>();
            appRunner.UseMicrosoftDependencyInjection(serviceProvider);

            string version = "v1.0.0.0";
            Console.WriteLine($"Running ASE-Dependency Checker {version}");

            return appRunner.Run(args);


        }
    }
}
