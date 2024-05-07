using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;


namespace FAHES.ERPIntegration.Inbound.Services
{
    public class DataSyncService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataSyncService> _logger;
        private ApiConsumer _consumer;
        
        public DataSyncService(IConfiguration configuration, ILogger<DataSyncService> logger, ApiConsumer consumer)
        {
            _configuration = configuration;  
            _logger = logger;
            _consumer = consumer;   
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                string BearerToken = await _consumer.GetBearerToken(_configuration);
                // Fetch data from API
                var config = _configuration.Get<AppConfig>();
                
                string empData = await _consumer.GetJsonAsync($"{config.ApiUrl}/GetFahesEmployees", BearerToken);

                Console.WriteLine(empData);

                    _logger.LogInformation(empData);
                    // Store data in SQL Server using Dapper
                   // await StoreEmployeeInDatabase(empData); 

                    string customerCreditData= await _consumer.GetJsonAsync($"{config.ApiUrl}/GetWoqodCustomerCreditLimit", BearerToken);
                //await StoreCustomerCreditInDatabase(customerCreditData);
                Console.WriteLine("customerCreditData");
                Console.WriteLine(customerCreditData);
                    _logger.LogInformation(customerCreditData);

                    // Wait for some time before next iteration (e.g., 1 hour)
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log any exceptions
                    _logger.LogError(ex, "An error occurred while fetching and storing data.");
                throw;
                }
                finally
                {
                    if (!stoppingToken.IsCancellationRequested)
                        _logger.LogError("Worker stopped unexpectedly");

                }

            }
        }

     

        private async Task StoreEmployeeInDatabase(string jsonData)
        {
            var config = _configuration.Get<AppConfig>();
            using (var connection = new SqlConnection(config.ConnectionStrings.DefaultConnection))
            {
                // Open connection
                await connection.OpenAsync();

                // Execute the stored procedure
                connection.ExecuteAsync("SP_ERP_InsertOrUpdateEmployee", new { json = jsonData }, commandType: CommandType.StoredProcedure);
            }
        }

        private async Task StoreCustomerCreditInDatabase(string jsonData)
        {
            var config = _configuration.Get<AppConfig>();
            using (var connection = new SqlConnection(config.ConnectionStrings.DefaultConnection))
            {
                // Open connection
                await connection.OpenAsync();

                // Execute the stored procedure
                connection.ExecuteAsync("SP_ERP_InsertOrUpdateCustomerCredit", new { json = jsonData }, commandType: CommandType.StoredProcedure);
            }
        }
    }


}

