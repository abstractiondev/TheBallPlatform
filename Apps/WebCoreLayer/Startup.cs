using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using Process = System.Diagnostics.Process;

namespace WebCoreLayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookieOptions =>
                {
                    cookieOptions.Cookie.Name = "THEBALL_AUTH";
                })
                .AddGoogle(googleOptions =>
                {
                    var clientId = SecureConfig.Current.GoogleOAuthClientID;
                    googleOptions.ClientId = clientId;
                    var clientSecret = SecureConfig.Current.GoogleOAuthClientSecret;
                    googleOptions.ClientSecret = clientSecret;
                })
                .AddFacebook(facebookOptions =>
                {
                    var clientId = SecureConfig.Current.FacebookOAuthClientID;
                    facebookOptions.ClientId = clientId;
                    var clientSecret = SecureConfig.Current.FacebookOAuthClientSecret;
                    facebookOptions.ClientSecret = clientSecret;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            initializePlatform();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseInformationContext();
            app.UseAuthentication();
            //app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }

        private void initializePlatform()
        {
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;

            string initedPath = ensureXDrive();

            var infraDriveRoot = initedPath ?? Environment.GetEnvironmentVariable("TBCoreFolder") ?? @"X:\";

            /*
            var infraDriveRoot = DriveInfo.GetDrives().Any(drive => drive.Name.StartsWith("X"))
                ? @"X:\"
                : Environment.GetEnvironmentVariable("TBCoreFolder");
           */
            string infraConfigFullPath = Path.Combine(infraDriveRoot, @"Configs\InfraShared\InfraConfig.json");
            RuntimeConfiguration.UpdateInfraConfig(infraConfigFullPath).Wait();

            var instances = InfraSharedConfig.Current.InstanceNames;
            foreach (var instance in instances)
            {
                RuntimeConfiguration.UpdateInstanceConfig(instance).Wait();
            }
        }


        private static string ensureXDrive()
        {
            bool hasDriveX = DriveInfo.GetDrives().Any(item => item.Name.ToLower().StartsWith("x"));
            //bool hasDriveX = false;
            if (!hasDriveX)
            {
                var infraAccountName = "";// CloudConfigurationManager.GetSetting("CoreFileShareAccountName");
                var infraAccountKey = ""; //CloudConfigurationManager.GetSetting("CoreFileShareAccountKey");
                bool isCloud = infraAccountName != null && infraAccountKey != null;
                if (isCloud)
                {
                    var netPath = Path.Combine(Environment.SystemDirectory, "net.exe");
                    //var args = $@"use X: \\{infraAccountName}.file.core.windows.net\tbcore /u:{infraAccountName} {infraAccountKey}";
                    string sharedLocation = $@"\\{infraAccountName}.file.core.windows.net\tbcore";
                    var args = $@"use {sharedLocation} /u:{infraAccountName} {infraAccountKey}";
                    var startInfo = new ProcessStartInfo(netPath) { UseShellExecute = false, Arguments = args };
                    var netProc = new Process { StartInfo = startInfo };
                    netProc.Start();
                    netProc.WaitForExit();
                    return sharedLocation;
                }
            }
            return null;
        }

    }
}
