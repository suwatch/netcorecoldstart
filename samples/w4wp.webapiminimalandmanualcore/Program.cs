using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.WebListener;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Server;

namespace SelfHostServer
{
    public class Startup
    {
        static string _path;
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var manager = new ApplicationPartManager();
            manager.FeatureProviders.Add(new StaticControllerFeatureProvider());

            services.AddSingleton(manager);
            
            services.AddMvcCore().AddJsonFormatters();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            _path = Environment.ExpandEnvironmentVariables(@"%windir%\System32\inetsrv\w4wp.webapiminimalandmanualcore");
            
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                var logFile = Path.Combine(_path, "logfile.log");
                File.AppendAllLines(logFile, new[] { string.Format("{0} {1}", DateTime.UtcNow.ToString("o"), ex) });
                Console.WriteLine(ex);
            }
        }
            
        public static void Run(string[] args)
        {
            var host = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseWebListener(options =>
                {
                    options.ListenerSettings.Authentication.Schemes = AuthenticationSchemes.None;
                    options.ListenerSettings.Authentication.AllowAnonymous = true;
                })
                .UseUrls($"http://{args[1]}:80")
                .Build();

            using (var sem = new Semaphore(0, 1, $"{args[1]}"))
            {
                sem.Release();
                host.Run();
            }
        }
        
        private class StaticControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
        {
            public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
            {
                feature.Controllers.Add(typeof(WebApiNetCore.Controllers.ValuesController).GetTypeInfo());
            }
        }
    }
}
