using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(InventoryService.Startup))]
namespace InventoryService
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureSettings(builder);
            //builder.Services.AddSingleton<IInventoryUpdator, InventoryUpdator>();
        }

        private void ConfigureSettings(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();
            _configuration = config;

            var connectionString = _configuration["Values:SqlConnectionString"];
            builder.Services.AddSingleton<IInventoryUpdator>(new InventoryUpdator(connectionString));
        }
    }
}