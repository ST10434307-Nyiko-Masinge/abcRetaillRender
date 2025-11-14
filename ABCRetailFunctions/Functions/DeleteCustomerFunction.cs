using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions;

public class DeleteCustomerFunction
{
    private readonly ILogger<DeleteCustomerFunction> _logger;

    public DeleteCustomerFunction(ILogger<DeleteCustomerFunction> logger)
    {
        _logger = logger;
    }

    [Function("DeleteCustomerFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}