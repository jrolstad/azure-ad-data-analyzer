using azuread_data_analyzer.Managers;
using azuread_data_analyzer.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace azuread_data_analyzer.console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var configuration = DependencyInjectionConfig.Configure();
                var serviceCollection = new ServiceCollection();

                DependencyInjectionConfig.Register(configuration, serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var manager = serviceProvider.GetService<DataManager>();

                var task = manager.ProcessApplications(Console.Out);
                //var task = manager.ProcessServicePrincipals(Console.Out, ServicePrincipalTypes.Application);
                //var task = manager.ProcessServicePrincipals(Console.Out, ServicePrincipalTypes.Legacy);
                //var task = manager.ProcessServicePrincipals(Console.Out, ServicePrincipalTypes.ManagedIdentity);
                //var task = manager.ProcessApplicationOwners(Console.Out);
                //var task = manager.ProcessServicePrincipalOwners(Console.Out, ServicePrincipalTypes.Application);
                //var task = manager.ProcessServicePrincipalOwners(Console.Out, ServicePrincipalTypes.Legacy);
                //var task = manager.ProcessServicePrincipalAppRoleAssignments(Console.Out, ServicePrincipalTypes.Application);
                //var task = manager.ProcessServicePrincipalAppRoleAssignments(Console.Out, ServicePrincipalTypes.Legacy);

                await Task.WhenAll(task);
                Console.WriteLine("Complete");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
