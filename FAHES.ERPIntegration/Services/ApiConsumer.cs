using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAHES.ERPIntegration.Inbound.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class ApiConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public ApiConsumer(ILogger<ApiConsumer> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;

        }

        public async Task<string> GetJsonAsync(string url, string bearerToken)
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //_httpClient.DefaultRequestHeaders.Authorization =
                    //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");


                    HttpResponseMessage response = await client.PostAsync(url, null);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Successfully retrieved data from {url}");
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        _logger.LogError($"Error retrieving data from {url}. Status code: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }


        public async Task<string> GetBearerToken(IConfiguration _configuration)
        {
            var config = _configuration.Get<AppConfig>();
            var tokenUrl = "https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/token";
            var tenantId = config.TenantId;
            var clientId = config.ClientId;
            var clientSecret = config.ClientSecret;

            var payload = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("scope", config.Scope) // Adjust scope as needed
        });


            try
            {

                var response = await _httpClient.PostAsync(tokenUrl.Replace("{tenant_id}", tenantId), payload);
                response.EnsureSuccessStatusCode(); // Throw on error response.

                var responseBody = await response.Content.ReadAsStringAsync();
                var accessToken = System.Text.Json.JsonDocument.Parse(responseBody).RootElement.GetProperty("access_token").GetString();
                _logger.LogInformation($"Token:{accessToken}");
                return accessToken;
            }
            catch (HttpRequestException e)
            {


                _logger.LogInformation($"Error while getting token  {e.Message}");
                throw;
            }



        }

    }
}
