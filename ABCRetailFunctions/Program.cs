using ABCRetailFunctions.Services;
using ABCRetailFunctions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
 Code attribution  
 this code was adapted from:
 Microsoft Learn – Azure Architecture Center  
 https://learn.microsoft.com/en-us/azure/architecture/example-scenario/  
 Accessed 7 October 2025  
*/



var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Register TableStorageServices using a factory to provide constructor params
        services.AddSingleton<TableStorageServices>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["storageConnectionString"];
            var tableName = "Customers"; // or read from configuration if needed
            return new TableStorageServices(connectionString!, tableName);
        });
    })
    .Build();

host.Run();
 






//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var host = new HostBuilder()
//    .ConfigureFunctionsWebApplication()
//    .ConfigureServices(services =>
//    {
//        services.AddApplicationInsightsTelemetryWorkerService();
//        services.ConfigureFunctionsApplicationInsights();
//    })
//    .Build();

//host.Run();
