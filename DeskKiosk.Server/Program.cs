using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DeskKiosk.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 5000);
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Disable CORS (allow all origins, headers, and methods)
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthorization();

            app.MapControllers();

            const string APP_URL = "http://localhost:5000";

            app.MapFallbackToFile("/index.html");
            try
            {

                Parallel.Invoke(
                    () => app.Run(),
                    () => OpenBrowserInIncognitoWithoutToolbars(APP_URL)
                );
            }
            catch (Exception)
            {
                try
                {
                    TerminateProcessUsingPort(5000);
                    TerminateProcessUsingPort(5173);
                    Parallel.Invoke(
                        () => app.Run(),
                        () => OpenBrowserInIncognitoWithoutToolbars(APP_URL)
                    );
                }
                catch (Exception)
                {
                    OpenBrowserInIncognitoWithoutToolbars(APP_URL);
                }
            }
        }

        static void TerminateProcessUsingPort(int port)
        {
            // Get all processes that are using the specified port
            var processes = Process.GetProcessesByName("dotnet")
                .Where(p => GetProcessPort(p) == port);

            foreach (var process in processes)
            {
                try
                {
                    // Terminate the process
                    process.Kill();
                    Console.WriteLine($"Terminated process with ID {process.Id} using port {port}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error terminating process: {ex.Message}");
                }
            }
        }


        static int GetProcessPort(Process process)
        {
            // You may need to adjust this logic based on your application's specifics
            try
            {
                return process.Modules
                    .Cast<ProcessModule>()
                    .Where(module => module.ModuleName.ToLower() == "myapplication.dll") // Replace with your application's DLL name
                    .Select(module => GetPortFromUrl(module.FileName))
                    .FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        static int GetPortFromUrl(string url)
        {
            // You may need to adjust this logic based on your application's specifics
            var match = Regex.Match(url, @":(\d+)/");
            if (match.Success && match.Groups.Count > 1)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return 0;
        }

        public static void OpenBrowserInIncognitoWithoutToolbars(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "chrome.exe",
                    Arguments = $"--incognito --app={url} --window-size=800,600 --disable-extensions --disable-infobars --disable-session-crashed-bubble --disable-toolbars", // Adjust window size as needed
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("google-chrome", $"--incognito --app={url} --window-size=800,600 --disable-extensions --disable-infobars --disable-session-crashed-bubble --disable-toolbars");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", $"-a 'Google Chrome' --args --incognito --app={url} --window-size=800,600 --disable-extensions --disable-infobars --disable-session-crashed-bubble --disable-toolbars");
            }
            else
            {
                // Handle unsupported platform
            }
        }
    }
}
