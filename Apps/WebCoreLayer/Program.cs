using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
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
            var certPath = @"X:\TheBallCerts\wildcard_theball.me.pfx";
            if (!File.Exists(certPath) && Startup.PlatformCoreRootPath != null)
                certPath = Path.Combine(Startup.PlatformCoreRootPath, "TheBallCerts", "wildcard_theball.me.pfx");
            bool useHttps = !String.IsNullOrEmpty(certPassword) && File.Exists(certPath);
            useHttps = false;
            var port = useHttps ? 10443 : 80;

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
                    //options.Listen(IPAddress.Any, 80);
                    options.Listen(IPAddress.Any, port, listenOptions =>
                    {
                        if (useHttps)
                        {
                            //listenOptions.UseHttps(certPath, certPassword);
                            listenOptions.UseHttps(new HttpsConnectionAdapterOptions()
                            {
                                ClientCertificateMode = ClientCertificateMode.NoCertificate,
                                ServerCertificate = new X509Certificate2(certPath, certPassword),
                                SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12,
                                CheckCertificateRevocation = false
                            });
                        }
                    });
                })
                .Build();
            return result;
        }
}
}
