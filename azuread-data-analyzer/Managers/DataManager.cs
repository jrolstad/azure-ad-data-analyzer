using azuread_data_analyzer.Services;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Managers
{
    public class DataManager
    {
        private readonly IDataStorageService _dataStorageService;
        private readonly ApplicationService _applicationService;
        private readonly ServicePrincipalService _servicePrincipalService;

        public DataManager(IDataStorageService dataStorageService,
            ApplicationService applicationService,
            ServicePrincipalService servicePrincipalService)
        {
            _dataStorageService = dataStorageService;
            _applicationService = applicationService;
            _servicePrincipalService = servicePrincipalService;
        }

        public Task ProcessApplications(TextWriter statusWriter)
        {
            return _applicationService.Get(async (data,pageCount) => await InsertData("Applications", data, statusWriter,pageCount));
        }

        public Task ProcessServicePrincipals(TextWriter statusWriter, string servicePrincipalType)
        {
            return _servicePrincipalService.Get(async (data, pageCount) => await InsertData("ServicePrincipals", data, statusWriter, pageCount), servicePrincipalType);
        }

        private async Task InsertData<T>(string destination,IEnumerable<T> data, TextWriter statusWriter, int pageCount) where T: Entity
        {
            try
            {
                statusWriter.WriteLine($"Inserting {destination} data for page {pageCount}");
                await _dataStorageService.Insert(destination, data);
            }
            catch(Exception ex)
            {
                var identitifiers = string.Join(",", data?.Select(d => d.Id));

                var exception = new Exception("Error when inserting data",ex);
                exception.Data.Add("destination", destination);
                exception.Data.Add("identitifiers", identitifiers);

                throw exception;
            }
        }
    }
}
