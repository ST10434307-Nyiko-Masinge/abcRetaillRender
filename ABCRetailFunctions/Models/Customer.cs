#nullable enable 
using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailFunctions.Models
{
    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Customer details
        public string PhotoURL { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public Customer() { }

        public Customer(string rowKey, string name, string surname, string email, string phoneNumber, string photoURL = "")
        {
            PartitionKey = "Customer";
            RowKey = rowKey;
            Name = name;
            Surname = surname;
            Email = email;
            PhoneNumber = phoneNumber;
            PhotoURL = photoURL;
        }
    }
}
