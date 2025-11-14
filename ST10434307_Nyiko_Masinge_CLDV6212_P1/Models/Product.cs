using Azure;
using Azure.Data.Tables;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Models
{
    public class Product : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Product details
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public Product() { }

        public Product(string rowKey, string name, string description, decimal price, int stockQuantity, string category, string imageURL = "")
        {
            PartitionKey = "Product";
            RowKey = rowKey;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            Category = category;
            ImageURL = imageURL;
            IsActive = true;
        }
    }
}

