using Azure;
using Azure.Data.Tables;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage
{
    public class ProductStorageService
    {
        private readonly TableClient tableClient;

        public ProductStorageService(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            tableClient = serviceClient.GetTableClient(tableName);
            tableClient.CreateIfNotExists();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task<List<Product>> GetActiveProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in tableClient.QueryAsync<Product>(p => p.IsActive == true))
            {
                products.Add(product);
            }
            return products;
        }

        public async Task<Product?> GetProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await tableClient.GetEntityAsync<Product>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            await tableClient.AddEntityAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await tableClient.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}

