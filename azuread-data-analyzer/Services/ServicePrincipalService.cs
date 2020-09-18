using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Services
{
    public class ServicePrincipalService
    {
        private readonly Factories.GraphClientFactory _graphClientFactory;

        public ServicePrincipalService(Factories.GraphClientFactory graphClientFactory)
        {
            _graphClientFactory = graphClientFactory;
        }

        public async Task Get(Action<ICollection<ServicePrincipal>,int> pageAction=null)
        {
            var client = _graphClientFactory.Create();

            var request = client.ServicePrincipals
                .Request()
                .Select("id,appId,displayName,signInAudience,servicePrincipalType,accountEnabled,appRoleAssignmentRequired,tags,appRoles")
                .Top(999);
            int pageNumber = 1;
            do
            {
                var result = await request.GetAsync();

                pageAction?.Invoke(result.CurrentPage, pageNumber);

                request = result.NextPageRequest;
                pageNumber++;

            } while (request != null);

        }
    }
}
