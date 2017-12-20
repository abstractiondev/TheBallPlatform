using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebCoreLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }



        public static IWebHost BuildWebHost(string[] args)
        {
            var certPassword =
                Environment.GetEnvironmentVariable("TBCertificatePassword");

            var result = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    /*
                    var certPath = @"X:\TheBallCerts\wildcard_theball.me.pfx";
                    if (!File.Exists(certPath))
                        certPath = Path.Combine(Startup.PlatformCoreRootPath, "TheBallCerts", "wildcard_theball.me.pfx");
                    //options.Listen(IPAddress.Loopback, 5000);
                    //options.UseHttps()
                    options.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.UseHttps(certPath, certPassword);
                    });*/
                    options.Listen(IPAddress.Any, 80);
                })
                .Build();
            return result;
        }
}
}
