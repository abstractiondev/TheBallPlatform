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
            var certPassword = Environment.GetEnvironmentVariable("TBCertificatePassword", EnvironmentVariableTarget.User);
            var result = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    //options.Listen(IPAddress.Loopback, 5000);
                    //options.UseHttps()
                    options.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.UseHttps(@"X:\TheBallCerts\wildcard_theball.me.pfx", certPassword);
                    });
                })
                .Build();
            return result;
        }
}
}
