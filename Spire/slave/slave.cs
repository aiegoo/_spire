using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;

namespace slave
{
    public class Program
    {
        public static void Main(string[] args)
        {

            if(!System.IO.File.Exists("secret.txt")){
                Console.WriteLine("Could not find secret.txt file...");
                Environment.Exit(1);
            }

			if(!System.IO.Directory.Exists("Scripts")){
				Console.WriteLine("Scripts directory does not exist...");
				Environment.Exit(1);
			}

            CreateHostBuilder(args).Build().Run();
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Listen(IPAddress.Parse("0.0.0.0"), 10000);
                    })
                    .UseStartup<Startup>();
                });
    }
}
