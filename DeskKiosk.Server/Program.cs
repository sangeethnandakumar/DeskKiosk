using DeskKiosk.Server.Infrastructure;
using System.Net;

namespace DeskKiosk.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = @"C:\Users\Sangeeth Nandakumar\source\repos\DeskKiosk\deskkiosk.client\dist"
            });
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 5000);
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<SignalRHub>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(builder => builder
                .WithOrigins("http://localhost:5173", "http://127.0.0.1:5000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");
            app.MapHub<SignalRHub>("/signalrhub");

            app.Run();
        }
    }
}
