using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ABCRetailFunctions.Models;
using ABCRetailFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions
{
    public class GetCustomersFunction
    {
        private readonly ILogger<GetCustomersFunction> _logger;
        private readonly TableStorageServices _tableStorageServices;

        // Inject your TableStorageServices via constructor

        public GetCustomersFunction(ILogger<GetCustomersFunction> logger, TableStorageServices tableStorageServices)
        {
            _logger = logger;
            _tableStorageServices = tableStorageServices;
        }


        //public GetCustomersFunction(ILogger<GetCustomersFunction> logger, TableStorageServices tableStorageServices)
        //{
        //    _logger = logger;
        //    _tableStorageServices = tableStorageServices;
        //}

        [Function("GetCustomers")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("Processing request to get customer(s) from Azure Table Storage.");

            // Extract optional customerId from query
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string? customerId = query["customerId"];

            // Retrieve all customers
            var customers = await _tableStorageServices.GetCustomersAsync();

            if (customers == null || customers.Count == 0)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteAsJsonAsync(new { message = "No customers found in storage." });
                return notFound;
            }

            // If specific customerId requested, filter it
            if (!string.IsNullOrEmpty(customerId))
            {
                var customer = customers.FirstOrDefault(c => c.RowKey == customerId);
                if (customer == null)
                {
                    var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFound.WriteAsJsonAsync(new { message = $"Customer with ID '{customerId}' not found." });
                    return notFound;
                }

                var singleResponse = req.CreateResponse(HttpStatusCode.OK);
                await singleResponse.WriteAsJsonAsync(customer);
                return singleResponse;
            }

            // Otherwise return all customers
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);
            return response;
        }
    }
}
