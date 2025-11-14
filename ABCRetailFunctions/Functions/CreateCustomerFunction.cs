using ABCRetailFunctions.Models;
using ABCRetailFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ABCRetailFunctions.Functions
{

    /*
 Code attribution  
 This code was adapted from Microsoft Learn  
 https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table  
 Accessed 7 October 2025  
*/

    public class CreateCustomerFunction
    {
        private readonly ILogger<CreateCustomerFunction> _logger;
        private readonly TableStorageServices _tableStorageServices;

        public CreateCustomerFunction(ILogger<CreateCustomerFunction> logger, TableStorageServices tableStorageServices)
        {
            _logger = logger;
            _tableStorageServices = tableStorageServices;
        }

        [Function("CreateCustomerFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Processing request to create a new customer.");

            // Read the customer from the request body
            var customer = await req.ReadFromJsonAsync<Customer>();
            if (customer == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid customer data.");
                return badRequest;
            }

            // Add customer to Table Storage
            var added = await _tableStorageServices.AddCustomerAsync(customer);

            var response = req.CreateResponse(added ? HttpStatusCode.OK : HttpStatusCode.Conflict);
            await response.WriteAsJsonAsync(customer);
            return response;
        }
    }
}
