using ABCRetailFunctions.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ABCRetailFunctions.Services
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
