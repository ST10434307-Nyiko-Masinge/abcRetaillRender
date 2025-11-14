using ABCRetailFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions;

public class GetCustomerFunction
{


    //private readonly ILogger<GetCustomersFunction> _logger;
    private readonly TableStorageServices _tableStorageServices;

    // Inject your TableStorageServices via constructor

    public GetCustomerFunction(ILogger<GetCustomersFunction> logger, TableStorageServices tableStorageServices)
    {
        _logger = (ILogger<GetCustomerFunction>?)logger;
        _tableStorageServices = tableStorageServices;
    }







    private readonly ILogger<GetCustomerFunction> _logger;

    public GetCustomerFunction(ILogger<GetCustomerFunction> logger)
    {
        _logger = logger;
    }

    [Function("GetCustomer")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}