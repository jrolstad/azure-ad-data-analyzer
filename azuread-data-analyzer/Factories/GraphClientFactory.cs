using azuread_data_analyzer.Models;
using azuread_data_analyzer.Services;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Factories
{
    public class GraphClientFactory
    {

        public GraphClientFactory(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        private GraphServiceClient _instance;
        private readonly ConfigurationService _configurationService;

        public GraphServiceClient Create()
        {
            if (_instance != null) return _instance;

            var graphServiceClient = new GraphServiceClient("https://graph.microsoft.com/beta/",
               new DelegateAuthenticationProvider(async requestMessage =>
               {
                   var tenantId = _configurationService.Get(ConfigurationNames.AzureAdTenantId);
                   var clientId = _configurationService.Get(ConfigurationNames.AzureAdClientId);
                   var clientSecret = _configurationService.Get(ConfigurationNames.AzureAdClientSecret);
                   var token = await GetAccessToken(tenantId, clientId, clientSecret, "https://graph.microsoft.com/.default");
                   requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
               }));


            return graphServiceClient;
        }

        private static async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret, params string[] scopes)
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            var result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();

            return result.AccessToken;
        }
    }
}
