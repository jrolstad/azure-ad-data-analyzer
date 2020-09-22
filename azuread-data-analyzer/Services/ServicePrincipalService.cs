using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task GetOwners(Action<ICollection<ServicePrincipal>, int> pageAction = null,string servicePrincipalType=null)
        {
            var client = _graphClientFactory.Create();

            var request = client.ServicePrincipals
                .Request()
                .Select("id")
                .Expand("owners")
                .Top(999);

            if (!string.IsNullOrWhiteSpace(servicePrincipalType))
            {
                request = request.Filter($"servicePrincipalType eq '{servicePrincipalType}'");
            }

            int pageNumber = 1;
            do
            {
                var result = await request.GetAsync();

                pageAction?.Invoke(result.CurrentPage, pageNumber);

                request = result.NextPageRequest;
                pageNumber++;

            } while (request != null);
        }

        public async Task GetAppRoleAssignments(Action<ICollection<ServicePrincipal>, int> pageAction = null, string servicePrincipalType = null)
        {
            var client = _graphClientFactory.Create();

            var request = client.ServicePrincipals
                .Request()
                .Select("id")
                .Expand("appRoleAssignedTo")
                .Top(999);

            if (!string.IsNullOrWhiteSpace(servicePrincipalType))
            {
                request = request.Filter($"servicePrincipalType eq '{servicePrincipalType}'");
            }

            int pageNumber = 1;
            do
            {
                var result = await request.GetAsync();
                pageAction?.Invoke(result.CurrentPage, pageNumber);

                request = result.NextPageRequest;
                pageNumber++;

            } while (request != null);
        }

        public async Task Get(Action<ICollection<ServicePrincipal>,int> pageAction=null, string servicePrincipalType = null)
        {
            var client = _graphClientFactory.Create();

            var request = client.ServicePrincipals
                .Request()
                .Select("id,appId,displayName,signInAudience,servicePrincipalType,accountEnabled,appRoleAssignmentRequired,tags,appRoles")
                .Top(999);

            if(!string.IsNullOrWhiteSpace(servicePrincipalType))
            {
                request = request.Filter($"servicePrincipalType eq '{servicePrincipalType}'");
            }
            
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
