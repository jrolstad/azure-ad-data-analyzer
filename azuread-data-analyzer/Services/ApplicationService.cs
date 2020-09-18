using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Services
{
    public class ApplicationService
    {
        private readonly Factories.GraphClientFactory _graphClientFactory;

        public ApplicationService(Factories.GraphClientFactory graphClientFactory)
        {
            _graphClientFactory = graphClientFactory;
        }
        public async Task Get(Action<ICollection<Application>> pageAction = null)
        {
            var client = _graphClientFactory.Create();

            var request = client.Applications
                .Request()
                .Select("id,appId,createdDateTime,description,displayName,signInAudience,tags,appRoles")
                .Top(999);

            do
            {
                var result = await request.GetAsync();

                pageAction?.Invoke(result.CurrentPage);
                break;

                request = result.NextPageRequest;

            } while (request != null);




        }
    }
}
