using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using System.Text.Json;



/* 
Code attribution
This code was adapted from Microsoft Learn
https://learn.microsoft.com/en-us/azure/storage/queues/storage-dotnet-how-to-use-queues
Accessed 6 October 2025
*/

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage
{
    
    public class QueueStorageServices
    {
        private readonly QueueClient queueClient;

        public QueueStorageServices(string storageConnectionString, string queueName)
        {
            var serviceClient = new QueueServiceClient(storageConnectionString);
            queueClient = serviceClient.GetQueueClient(queueName);
            queueClient.CreateIfNotExists();
        }

        //send maessage to queue 
        public async Task SendMessagesAsync(object message)
        {
            //convert message to object JSON string 
            var messageJson = JsonSerializer.Serialize(message);
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
        }
        //get messages for log from queue
        public async Task<List<QueueLogViewModel>> getMessagesAsync()
        {
            var messageList = new List<QueueLogViewModel>();
            var messagesz = await queueClient.ReceiveMessagesAsync(maxMessages: 25);
            foreach (QueueMessage message in messagesz.Value)
            {
                messageList.Add(new QueueLogViewModel
                {
                    MessageID = message.MessageId,
                    InsertionTime = message.InsertedOn, 
                    MessagesText = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()))
                });
            }

            return messageList;
            // public async Task

        }
    }
}