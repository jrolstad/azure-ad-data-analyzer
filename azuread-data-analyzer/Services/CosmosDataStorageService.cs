﻿using azuread_data_analyzer.Factories;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Services
{
    public class CosmosDataStorageService:IDataStorageService
    {
        private readonly CosmosClientFactory _clientFactory;
        private const string DatabaseId = "AzureAdCache";
        private IBulkExecutor _bulkExecutor = null;
        public CosmosDataStorageService(CosmosClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task Insert<T>(string destination, IEnumerable<T> data)
        {
            if (_bulkExecutor == null)
            {
                _bulkExecutor = await _clientFactory.Create(DatabaseId, destination);
            }

            var typedData = data.Cast<object>();

            await _bulkExecutor.BulkImportAsync(typedData, enableUpsert: true, disableAutomaticIdGeneration: true);
        }

    }
}
