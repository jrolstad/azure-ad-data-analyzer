using Microsoft.Extensions.Configuration;

namespace azuread_data_analyzer.Services
{
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Get(string name)
        {
            return _configuration[name];
        }
    }
}
