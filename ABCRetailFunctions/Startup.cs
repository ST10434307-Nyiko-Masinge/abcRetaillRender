using ABCRetailFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;

[assembly: FunctionsStartup(typeof(ABCRetailFunctions.Startup))]

namespace ABCRetailFunctions
{
    /*
 Code attribution  
 This code was adapted from Microsoft Learn  
 https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection  
 Accessed 7 October 2025  
*/

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) 
        {
            //Register tableStorage 
            builder.Services.AddSingleton(sp => CreateStorageService<TableStorageServices>(sp, "customer", "table"));
            //Register blobstorage 
            builder.Services.AddSingleton(sp => CreateStorageService<BlobStorageServices>(sp, "customer-pictures", "blob"));

            //Register fileshare 
            builder.Services.AddSingleton(sp => CreateStorageService<FileShareStorageServices>(sp, "ABC-log-file", "fileshare"));

            //Register queue
            builder.Services.AddSingleton(sp => CreateStorageService<QueueStorageServices>(sp, "abc-logs", "queue"));

        }

        private T CreateStorageService<T>(IServiceProvider sp, string serviceIdentifier, string serviceType)where T : class 
        {
            var logger = sp.GetRequiredService<ILogger<Startup>>();
            var storageConnectionString = Environment.GetEnvironmentVariable("storageConnectionString");

            if (string.IsNullOrEmpty(storageConnectionString) || string.IsNullOrWhiteSpace(serviceIdentifier))
            {
                logger.LogError("Storage Connection string or Service Identifier is not set");

                throw new InvalidOperationException("Configuration is invalid");
            }

            logger.LogInformation($"Using {serviceType} identifier: {serviceIdentifier}");

            return serviceType switch
            {
                "table" => new TableStorageServices(storageConnectionString, serviceIdentifier) as T,
                "blob" => new BlobStorageServices(storageConnectionString, serviceIdentifier) as T,
                "fileshare" => new FileShareStorageServices(storageConnectionString, serviceIdentifier) as T,
                "queue" => new QueueStorageServices(storageConnectionString, serviceIdentifier) as T,
                _=> throw new NotImplementedException($"{serviceType} is not supported")
            };

        }
    }
}
