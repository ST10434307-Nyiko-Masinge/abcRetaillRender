using Azure;
using Azure.Data.Tables;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage
{
   
    public class TableStorageServices
    {
        private readonly TableClient tableClient;

        public TableStorageServices(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            tableClient = serviceClient.GetTableClient(tableName);
            tableClient.CreateIfNotExists();
        }

        //use asyc tasks  and implement error handling(try catch)
        //get all customers index.cshtml;.
        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();
            await foreach (var customer in tableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        //get customer by rowkey details.cshtml
        public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await tableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null; // customer not found
            }
        }

        //add customer
        public async Task AddCustomerAsync(Customer customer) 
        {
            //set partitionKey and rowkey 
            await tableClient.AddEntityAsync(customer);
            //Add the student
            //await tableClient.AddEntityAsync(customer);
        }

        //edit student edit.cshtml 
        public async Task UpdateCustomerAsync(Customer customer)
        {
            

            //Add the student
            await tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
        }

        //delete cust cs.gtml
        public async Task DeleteCustomerAsync(string PartitionKey, string RowKey)
        {
            await tableClient.DeleteEntityAsync(PartitionKey, RowKey);
        }
    }
}
