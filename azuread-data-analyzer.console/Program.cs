using azuread_data_analyzer.Managers;
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

                var appTask = manager.ProcessApplications();
                var servicePrincipalTask = manager.ProcessServicePrincipals();

                await Task.WhenAll(appTask, servicePrincipalTask);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
            
        }
    }
}
