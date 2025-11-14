using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions;

public class UpdateCustomerFunction
{

    /*
 Code attribution  
 This code was adapted from Microsoft Learn  
 https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table  
 Accessed 7 October 2025  
*/

    private readonly ILogger<UpdateCustomerFunction> _logger;

    public UpdateCustomerFunction(ILogger<UpdateCustomerFunction> logger)
    {
        _logger = logger;
    }

    [Function("UpdateCustomerFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}