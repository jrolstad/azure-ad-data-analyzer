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

                //var appTask = manager.ProcessApplications(Console.Out);
                var servicePrincipalTask = manager.ProcessServicePrincipals(Console.Out, ServicePrincipalTypes.Legacy);

                await Task.WhenAll(servicePrincipalTask);

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
