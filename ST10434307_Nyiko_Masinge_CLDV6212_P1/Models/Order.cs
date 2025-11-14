using Azure;
using Azure.Data.Tables;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingPostalCode { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string OrderItemsJson { get; set; } = string.Empty; // Serialized list of CartItems
    }
}

