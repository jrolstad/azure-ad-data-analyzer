using azuread_data_analyzer.Services;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
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

        public Task ProcessApplications()
        {
            return _applicationService.Get(async d => await InsertData("Applications", d));
        }

        public Task ProcessServicePrincipals()
        {
            return _servicePrincipalService.Get(async d => await InsertData("ServicePrincipals", d));
        }

        private async Task InsertData<T>(string destination,IEnumerable<T> data) where T: Entity
        {
            try
            {
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
