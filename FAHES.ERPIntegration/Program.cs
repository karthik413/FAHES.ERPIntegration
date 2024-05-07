using FAHES.ERPIntegration.Inbound.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
 
using Serilog;
using System.Net.Http.Headers;

public class Program
{
    public static void Main(string[] args)
    {


        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File($"ERPInboundapp.log")
        .CreateLogger();
        // Use Serilog for logging in the console
        Log.Information("Starting up the application");
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }



    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, configuration) =>
        {
            configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .UseSerilog() 
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DataSyncService>();
                services.AddTransient<ApiConsumer>();
               

            })
        ;
            
}
