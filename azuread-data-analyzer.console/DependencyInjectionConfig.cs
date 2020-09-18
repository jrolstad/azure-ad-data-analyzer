using azuread_data_analyzer.Factories;
using azuread_data_analyzer.Managers;
using azuread_data_analyzer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace azuread_data_analyzer.console
{
    public static class DependencyInjectionConfig
    {
        public static IConfiguration Configure()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration;
        }

        public static void Register(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton(provider => { return configuration; });

            services.AddTransient<CosmosClientFactory>();
            services.AddTransient<GraphClientFactory>();

            services.AddTransient<DataManager>();

            services.AddTransient<ApplicationService>();
            services.AddTransient<ServicePrincipalService>();
            services.AddTransient<ConfigurationService>();

            //services.AddTransient<IDataStorageService,CosmosDataStorageService>();
            services.AddTransient<IDataStorageService,SqlDataStorageService>();



        }
    }
}
