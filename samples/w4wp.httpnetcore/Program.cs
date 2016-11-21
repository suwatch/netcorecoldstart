using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Server;

namespace HelloWorld
{
    public class Program
    {
        static string _path;
        
        public static void Main(string[] args)
        {
            _path = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\w4wp.httpnetcore");
            
            try
            {
                Run(args).Wait();
            }
            catch (Exception ex)
            {
                var logFile = Path.Combine(_path, "logfile.log");
                File.AppendAllLines(logFile, new[] { string.Format("{0} {1}", DateTime.UtcNow.ToString("o"), ex) });
                Console.WriteLine(ex);
            }
        }
        
        public static async Task Run(string[] args)
        {
            var settings = new WebListenerSettings();
            settings.UrlPrefixes.Add($"http://{args[1]}:80");

            using (WebListener listener = new WebListener(settings))
            {                
                using (var sem = new Semaphore(0, 1, $"{args[1]}"))
                {
                    sem.Release();
                    listener.Start();
                }
                
                Console.WriteLine("{0} started", DateTime.UtcNow.ToString("o"));
                while (true)
                {
                    RequestContext context = await listener.AcceptAsync();
                    Console.WriteLine("{0} accepted", DateTime.UtcNow.ToString("o"));

                    // Response
                    var html = Path.Combine(_path, "index.html");
                    var bytes = File.ReadAllBytes(html);

                    //StringBuilder strb = new StringBuilder();           
                    //foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                    //{
                    //    strb.AppendLine(module.FileName);
                    //}
                    //bytes = Encoding.UTF8.GetBytes(strb.ToString());

                    //Console.WriteLine("Hello World");
                    context.Response.ContentLength = bytes.Length;
                    context.Response.ContentType = "text/plain";

                    context.Response.Body.Write(bytes, 0, bytes.Length);
                    context.Dispose();
                }
            }
        }
    }
}