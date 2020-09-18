using azuread_data_analyzer.Models;
using azuread_data_analyzer.Services;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Factories
{
    public class CosmosClientFactory
    {
        private readonly ConfigurationService _configurationService;

        public CosmosClientFactory(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }
        public DocumentClient CreateClient()
        {
            var key = _configurationService.Get(ConfigurationNames.CosmosDocumentKey);
            var url = _configurationService.Get(ConfigurationNames.CosmosUrl);

            return new DocumentClient(new Uri(url), key);
        }

        public async Task<IBulkExecutor> Create(string databaseId, string collectionId)
        {
            var client = CreateClient();
            var collection = client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(databaseId))
               .Where(c => c.Id == collectionId)
               .AsEnumerable()
               .FirstOrDefault();

            var executor = new BulkExecutor(client, collection);
            await executor.InitializeAsync();

            return executor;
        }
    }
}
