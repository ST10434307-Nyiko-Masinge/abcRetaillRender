using Azure;
using Azure.Data.Tables;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage
{
    public class OrderStorageService
    {
        private readonly TableClient tableClient;

        public OrderStorageService(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            tableClient = serviceClient.GetTableClient(tableName);
            tableClient.CreateIfNotExists();
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();
            await foreach (var order in tableClient.QueryAsync<Order>())
            {
                orders.Add(order);
            }
            return orders;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = new List<Order>();
            await foreach (var order in tableClient.QueryAsync<Order>(o => o.UserId == userId))
            {
                orders.Add(order);
            }
            return orders;
        }

        public async Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await tableClient.GetEntityAsync<Order>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task AddOrderAsync(Order order)
        {
            await tableClient.AddEntityAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await tableClient.UpdateEntityAsync(order, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteOrderAsync(string partitionKey, string rowKey)
        {
            await tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}

