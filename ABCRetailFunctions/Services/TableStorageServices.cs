
#nullable enable
using ABCRetailFunctions.Models;
using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailFunctions.Services
{
    public class TableStorageServices
    {
        private readonly TableClient tableClient;

        public TableStorageServices(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            tableClient = serviceClient.GetTableClient(tableName);
            // This call should be synchronous in the constructor, but it's good practice
            // to ensure the table exists before attempting operations.
            tableClient.CreateIfNotExists();
        }

        // get all customers index.cshtml
        // Error handling is less critical here, as an empty list is the typical outcome of an empty table.
        // RequestFailedException (e.g., Auth failure) would be thrown if the service is unreachable.
        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();
            try
            {
                await foreach (var customer in tableClient.QueryAsync<Customer>())
                {
                    customers.Add(customer);
                }
            }
            catch (RequestFailedException ex)
            {
                // Log the exception here for production code
                Console.WriteLine($"Error retrieving customers: {ex.Message}");
                // Return an empty list on failure
                return new List<Customer>();
            }
            return customers;
        }

        // get customer by rowkey details.cshtml
        public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await tableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Customer not found
                return null;
            }
            catch (RequestFailedException ex)
            {
                // Log other request errors (e.g., auth, network)
                Console.WriteLine($"Error retrieving customer: {ex.Message}");
                return null;
            }
        }

        // add customer
        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            try
            {
                // set partitionKey and rowkey (Customer model handles the assignment)
                await tableClient.AddEntityAsync(customer);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 409) // Conflict (Entity already exists)
            {
                Console.WriteLine($"Error adding customer: Customer with Pk/Rk already exists. {ex.Message}");
                return false;
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
                return false;
            }
        }

        // edit student edit.cshtml 
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                // Use ETag.All for unconditional update (Replace mode)
                await tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404) // Not Found
            {
                Console.WriteLine($"Error updating customer: Customer not found. {ex.Message}");
                return false;
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        // delete cust cs.gtml
        public async Task<bool> DeleteCustomerAsync(string PartitionKey, string RowKey)
        {
            try
            {
                // The DeleteEntityAsync method is safe to call even if the entity doesn't exist 
                // when using ETag.All (default behavior). However, catching 404 provides clearer feedback.
                await tableClient.DeleteEntityAsync(PartitionKey, RowKey);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Customer not found, but we can treat this as a success (entity is gone)
                Console.WriteLine($"Customer to delete not found: {ex.Message}");
                return true;
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }
    }
}


























//#nullable enable
//using ABCRetailFunctions.Models;
//using Azure;
//using Azure.Data.Tables;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ABCRetailFunctions.Services
//{
//    public class TableStorageServices
//    {
//        private readonly TableClient tableClient;

//        public TableStorageServices(string storageConnectionString, string tableName)
//        {
//            var serviceClient = new TableServiceClient(storageConnectionString);
//            tableClient = serviceClient.GetTableClient(tableName);
//            tableClient.CreateIfNotExists();
//        }

//        //use asyc tasks  and implement error handling(try catch)
//        //get all customers index.cshtml;.
//        public async Task<List<Customer>> GetCustomersAsync()
//        {
//            var customers = new List<Customer>();
//            await foreach (var customer in tableClient.QueryAsync<Customer>())
//            {
//                customers.Add(customer);
//            }
//            return customers;
//        }

//        //get customer by rowkey details.cshtml
//        public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
//        {
//            try
//            {
//                var response = await tableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
//                return response.Value;
//            }
//            catch (RequestFailedException ex) when (ex.Status == 404)
//            {
//                return null; // customer not found
//            }
//        }

//        //add customer
//        public async Task AddCustomerAsync(Customer customer)
//        {
//            //set partitionKey and rowkey 
//            await tableClient.AddEntityAsync(customer);
//            //Add the student
//            //await tableClient.AddEntityAsync(customer);
//        }

//        //edit student edit.cshtml 
//        public async Task UpdateCustomerAsync(Customer customer)
//        {


//            //Add the student
//            await tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
//        }

//        //delete cust cs.gtml
//        public async Task DeleteCustomerAsync(string PartitionKey, string RowKey)
//        {
//            await tableClient.DeleteEntityAsync(PartitionKey, RowKey);
//        }
//    }
//}
