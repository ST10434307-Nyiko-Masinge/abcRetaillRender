using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using System.Net.Http.Json;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services
{
    public class FunctionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _functionsBaseUrl;

        public FunctionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _functionsBaseUrl = configuration["AzureFunctionsBaseUrlProd"] ?? "http://localhost:7230";
            _functionsBaseUrl = _functionsBaseUrl.TrimEnd('/');
        }

        // GET all customers
        public async Task<List<Customer>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Customer>>($"{_functionsBaseUrl}/api/GetCustomers");
                return response ?? new List<Customer>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error calling GetCustomers: {ex.Message}");
                return new List<Customer>();
            }
        }

        // GET single customer
        public async Task<Customer?> GetCustomerAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return null;

            try
            {
                return await _httpClient.GetFromJsonAsync<Customer>($"{_functionsBaseUrl}/api/GetCustomers?customerId={customerId}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting customer {customerId}: {ex.Message}");
                return null;
            }
        }



        // CREATE customer
        public async Task<bool> CreateCustomerAsync(Customer customer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_functionsBaseUrl}/api/CreateCustomerFunction", customer);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return false;
            }
        }



        //// CREATE customer
        //public async Task<bool> CreateCustomerAsync(Customer customer)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync($"{_functionsBaseUrl}/api/CreateCustomer", customer);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine($"Error creating customer: {ex.Message}");
        //        return false;
        //    }
        //}

        // UPDATE customer
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_functionsBaseUrl}/api/UpdateCustomer", customer);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        // DELETE customer
        public async Task<bool> DeleteCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_functionsBaseUrl}/api/DeleteCustomer?customerId={customerId}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }
    }
}






//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
//using System.Net.Http.Json;

//namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services
//{
//    public class FunctionService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _functionsBaseUrl;

//        public FunctionService(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;

//            // The URL of your running Azure Function App
//            // (e.g. http://localhost:7230 for local / or deployed URL in production)
//            _functionsBaseUrl = configuration["AzureFunctionsBaseUrl"] ?? "http://localhost:7230";
//            _functionsBaseUrl = _functionsBaseUrl.TrimEnd('/');
//        }

//        // 🟩 GET: All customers
//        public async Task<List<Customer>> GetCustomersAsync()
//        {
//            try
//            {
//                var response = await _httpClient.GetFromJsonAsync<List<Customer>>($"{_functionsBaseUrl}/api/GetCustomersFunctions");
//                return response ?? new List<Customer>();
//            }
//            catch (HttpRequestException ex)
//            {
//                Console.WriteLine($"❌ Error fetching customers: {ex.Message}");
//                return new List<Customer>();
//            }
//        }

//        // 🟦 GET: Single customer by ID
//        public async Task<Customer?> GetCustomerAsync(string customerId)
//        {
//            if (string.IsNullOrEmpty(customerId))
//                return null;

//            try
//            {
//                var response = await _httpClient.GetFromJsonAsync<Customer>(
//                    $"{_functionsBaseUrl}/api/GetCustomersFunctions?customerId={customerId}");
//                return response;
//            }
//            catch (HttpRequestException ex)
//            {
//                Console.WriteLine($"❌ Error fetching customer {customerId}: {ex.Message}");
//                return null;
//            }
//        }

//        // 🟨 POST: Create a new customer
//        public async Task<bool> CreateCustomerAsync(Customer newCustomer)
//        {
//            try
//            {
//                var response = await _httpClient.PostAsJsonAsync($"{_functionsBaseUrl}/api/CreateCustomerFunctions", newCustomer);
//                if (response.IsSuccessStatusCode)
//                {
//                    Console.WriteLine("✅ Customer created successfully.");
//                    return true;
//                }

//                var error = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"⚠️ Failed to create customer: {error}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error creating customer: {ex.Message}");
//                return false;
//            }
//        }

//        // 🟧 PUT: Update existing customer
//        public async Task<bool> UpdateCustomerAsync(Customer updatedCustomer)
//        {
//            try
//            {
//                var response = await _httpClient.PutAsJsonAsync($"{_functionsBaseUrl}/api/UpdateCustomerFunction", updatedCustomer);
//                if (response.IsSuccessStatusCode)
//                {
//                    Console.WriteLine("✅ Customer updated successfully.");
//                    return true;
//                }

//                var error = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"⚠️ Failed to update customer: {error}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error updating customer: {ex.Message}");
//                return false;
//            }
//        }

//        // 🟥 DELETE: Delete customer by ID
//        public async Task<bool> DeleteCustomerAsync(string customerId)
//        {
//            if (string.IsNullOrEmpty(customerId))
//                return false;

//            try
//            {
//                var response = await _httpClient.DeleteAsync($"{_functionsBaseUrl}/api/DeleteCustomerFunctions?customerId={customerId}");
//                if (response.IsSuccessStatusCode)
//                {
//                    Console.WriteLine("✅ Customer deleted successfully.");
//                    return true;
//                }

//                var error = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"⚠️ Failed to delete customer: {error}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error deleting customer: {ex.Message}");
//                return false;
//            }
//        }

//        // 📨 GET: Retrieve all messages from Queue
//        public async Task<List<string>> GetMessagesAsync()
//        {
//            try
//            {
//                var response = await _httpClient.GetFromJsonAsync<List<string>>($"{_functionsBaseUrl}/api/GetMessagesFunctions");
//                return response ?? new List<string>();
//            }
//            catch (HttpRequestException ex)
//            {
//                Console.WriteLine($"❌ Error fetching messages: {ex.Message}");
//                return new List<string>();
//            }
//        }

//        // 📂 POST: Upload log file to File Share
//        public async Task<bool> UploadLogFileAsync(byte[] fileBytes, string fileName)
//        {
//            try
//            {
//                var content = new MultipartFormDataContent();
//                var fileContent = new ByteArrayContent(fileBytes);
//                content.Add(fileContent, "file", fileName);

//                var response = await _httpClient.PostAsync($"{_functionsBaseUrl}/api/UploadLogFileFunction", content);
//                return response.IsSuccessStatusCode;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error uploading file: {ex.Message}");
//                return false;
//            }
//        }
//    }
//}
























//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
//using System.Net.Http.Json;

//namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services
//{
//    public class FunctionService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _functionsBaseUrl;

//        public FunctionService(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;

//            // Set your local Azure Function app URL here or via configuration
//            // Example: "http://localhost:7230" matches your Function host logs
//            _functionsBaseUrl = configuration["AzureFunctionsBaseUrl"] ?? "http://localhost:7230";
//            _functionsBaseUrl = _functionsBaseUrl.TrimEnd('/');
//        }

//        // Get all customers
//        public async Task<List<Customer>> GetCustomersAsync()
//        {
//            try
//            {
//                var response = await _httpClient.GetFromJsonAsync<List<Customer>>($"{_functionsBaseUrl}/api/GetCustomers");
//                return response ?? new List<Customer>();
//            }
//            catch (HttpRequestException ex)
//            {
//                Console.WriteLine($"Error calling GetCustomers function: {ex.Message}");
//                return new List<Customer>();
//            }
//        }

//        // Get single customer by RowKey (optional)
//        public async Task<Customer?> GetCustomerAsync(string customerId)
//        {
//            if (string.IsNullOrEmpty(customerId))
//                return null;

//            try
//            {
//                // Pass customerId as query parameter
//                var response = await _httpClient.GetFromJsonAsync<Customer>($"{_functionsBaseUrl}/api/GetCustomers?customerId={customerId}");
//                return response;
//            }
//            catch (HttpRequestException ex)
//            {
//                Console.WriteLine($"Error calling GetCustomers function for ID {customerId}: {ex.Message}");
//                return null;
//            }
//        }

//        // TODO: Add methods for CreateCustomer, UpdateCustomer, DeleteCustomer, etc.
//    }
//}

















//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;

//namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services
//{
//    public class FunctionService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _functionsBaseUrl;
//        public FunctionService(HttpClient httpClient, IConfiguration configuration) 
//        {
//            _httpClient = httpClient;
//            var rawBaseUrl = configuration["AzureFunctionsBaseUrl"] ?? throw new InvalidOperationException("Azure Functions base URL is missing");
//            _functionsBaseUrl = rawBaseUrl.TrimEnd('/');
//        }

//        //get all customer
//        public async Task<List<Customer>> GetCustomersAsync() 
//        { 
//            var response = await _httpClient.GetFromJsonAsync<List<Customer>>($"{_functionsBaseUrl}/api/GetCustomers");
//            return response ?? new List<Customer>();
//        }

//get customer details 

//create customer

//updatae customer 

//delete customer

//get all messages from the queue

//upload file to file share 

//    }
//}
