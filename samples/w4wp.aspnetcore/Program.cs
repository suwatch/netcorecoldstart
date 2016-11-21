using System;
using System.Diagnostics;
using System.IO;
//using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.WebListener;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Server;

namespace SelfHostServer
{
    public class Startup
    {
        static string _path;
        
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
        {
            //loggerfactory.AddConsole(LogLevel.Debug);

            app.Run(async context =>
            {
                var html = Path.Combine(_path, "index.html");
                var text = File.ReadAllText(html);
                
                if (context.Request.Path.Value != "/")
                {
                    StringBuilder strb = new StringBuilder();           
                    foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                    {
                        strb.AppendLine(module.FileName);
                    }
                    text = strb.ToString();
                }
                
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(text);
            });
        }

        public static void Main(string[] args)
        {
            _path = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\w4wp.aspnetcore");
            
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
                    //options.ListenerSettings.UrlPrefixes.Add($"http://{args[1]}:80");
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
    }
}
