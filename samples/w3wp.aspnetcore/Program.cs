using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SelfHostServer
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
        {
            //loggerfactory.AddConsole(LogLevel.Debug);

            app.Run(async context =>
            {
                var html = Path.Combine(Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\SiteExtensions\AspNetCore\1.0.0"), "index.html");
                var text = File.ReadAllText(html);

                //StringBuilder strb = new StringBuilder();           
                //foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                //{
                //    strb.AppendLine(module.FileName);
                //}
                //text = strb.ToString();
                
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(text);
            });
        }

        public static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                var logFile = Path.Combine(Environment.ExpandEnvironmentVariables(@"%home%\LogFiles"), "logfile.log");
                File.AppendAllLines(logFile, new[] { string.Format("{0} {1}", DateTime.UtcNow.ToString("o"), ex) });
                Console.WriteLine(ex);
            }
        }
            
        public static void Run(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
                
            host.Run();                
        }
    }
}
