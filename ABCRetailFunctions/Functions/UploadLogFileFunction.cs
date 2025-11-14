using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions;

public class UploadLogFileFunction
{
    private readonly ILogger<UploadLogFileFunction> _logger;

    // This is the dependency injection constructor and is fine.
    public UploadLogFileFunction(ILogger<UploadLogFileFunction> logger)
    {
        _logger = logger;
    }

    [Function("UploadLogFileFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        //  get the 'name' from query string (for GET or POST with a query string)
        string name = req.Query["name"];

        // 'name' is null or empty, read from  request body (for POST)
        if (string.IsNullOrEmpty(name) && req.ContentLength > 0)
        {
            try
            {
                // Read the request body as a string
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // Check if the body contains JSON data
                if (!string.IsNullOrEmpty(requestBody))
                {
                    // Deserialize the JSON to a simple object with a 'name' property
                    using (JsonDocument doc = JsonDocument.Parse(requestBody))
                    {
                        if (doc.RootElement.TryGetProperty("name", out JsonElement nameElement))
                        {
                            name = nameElement.GetString();
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error parsing JSON body: {ex.Message}");
                // Continue, allowing 'name' to remain null or empty
            }
        }

        // response message
        string welcomeMessage = string.IsNullOrEmpty(name)
            ? "Welcome to Azure Functions! Please provide a name in the query string (?name=...) or the request body."
            : $"Welcome, {name}, to Azure Functions!";

        return new OkObjectResult(welcomeMessage);
    }
}