using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE.Storage;

namespace TheBall
{
    public static class QueueSupport
    {
        public const string DefaultQueueName = "defaultqueue";
        public const string ErrorQueueName = "errorqueue";
        public const string StatisticQueueName = "statisticqueue";

        public static CloudQueue CurrDefaultQueue { get; private set; }
        public static CloudQueue CurrErrorQueue { get; private set; }
        public static CloudQueueClient CurrQueueClient { get; private set; }
        public static CloudQueue CurrStatisticsQueue { get; private set; }
        public static ConcurrentDictionary<string, CloudQueue> Queues = new ConcurrentDictionary<string, CloudQueue>();

        public static async Task InitializeAfterStorage(bool debugMode = false)
        {
            CurrQueueClient = StorageSupport.CurrStorageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            string dbgModePrefix = debugMode ? "dbg" : "";
            CloudQueue queue = CurrQueueClient.GetQueueReference(dbgModePrefix + DefaultQueueName);
            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();
            CurrDefaultQueue = queue;

            queue = CurrQueueClient.GetQueueReference(ErrorQueueName);
            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();
            CurrErrorQueue = queue;

            queue = CurrQueueClient.GetQueueReference(dbgModePrefix + StatisticQueueName);
            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();
            CurrStatisticsQueue = queue;
        }

        public static async Task ReportStatistics(string message, TimeSpan? toLive = null)
        {
            if(toLive.HasValue == false)
                toLive = TimeSpan.FromDays(7);
            await CurrStatisticsQueue.AddMessageAsync(new CloudQueueMessage(message), toLive.Value, null, null, null);
        }

        public static async Task<CloudQueue> RegisterQueue(string queueName, bool createIfNotExist = true)
        {
            var queue = CurrQueueClient.GetQueueReference(queueName);
            bool addResult = Queues.TryAdd(queueName, queue);
            if(addResult == false)
                throw new InvalidOperationException("Cannot add already existing queue: " + queueName);
            if (createIfNotExist)
                await queue.CreateIfNotExistsAsync();
            return queue;
        }

        public static CloudQueue UnregisterQueue(string queueName)
        {
            CloudQueue queue;
            bool removeResult = Queues.TryRemove(queueName, out queue);
            if(removeResult == false)
                throw new InvalidOperationException("Cannot remove non-existent queue: " + queueName);
            return queue;
        }

        public static CloudQueue GetQueue(string queueName)
        {
            CloudQueue queue;
            var getResult = Queues.TryGetValue(queueName, out queue);
            if(getResult == false)
                throw new InvalidOperationException("No queue with name registered: " + queueName);
            return queue;
        }

        public static async Task PutToErrorQueue(string error)
        {
            CloudQueueMessage message = new CloudQueueMessage(error);
            await CurrErrorQueue.AddMessageAsync(message);
        }

        public static async Task WriteObjectAsJSONToQueue<T>(string queueName, T objectToWrite)
        {
            var queue = GetQueue(queueName);
            var jsonString = JSONSupport.SerializeToJSONString(objectToWrite);
            CloudQueueMessage message = new CloudQueueMessage(jsonString);
            await queue.AddMessageAsync(message);
        }



        public static async Task<MessageObject<T>[]> GetJSONObjectsFromQueue<T>(string queueName, int maxMessagesToRetrieve)
        {
            if (maxMessagesToRetrieve > 32)
                throw new ArgumentException("Max messages to retrieve is 32", "maxMessagesToRetrieve");
            var queue = GetQueue(queueName);
            var messages = await queue.GetMessagesAsync(maxMessagesToRetrieve);
            List<MessageObject<T>> resultData = new List<MessageObject<T>>();
            foreach (var message in messages)
            {
                var jsonString = message.AsString;
                var contentObject = JSONSupport.GetObjectFromString<T>(jsonString);
                MessageObject<T> messageObject = new MessageObject<T>
                    {
                        Message = message,
                        RetrievedObject = contentObject
                    };
                resultData.Add(messageObject);
            }
            var messageObjects = resultData.ToArray();
            return messageObjects;
        }

        public class MessageObject<T>
        {
            public CloudQueueMessage Message;
            public T RetrievedObject;
        }

        public static async Task PutMessageToQueue(string queueName, string messageText)
        {
            var queue = GetQueue(queueName);
            CloudQueueMessage message = new CloudQueueMessage(messageText);
            await queue.AddMessageAsync(message);
        }

        public static async Task<MessageObject<string>[]> GetMessagesFromQueue(string queueName, int maxCount = 32, bool deleteOnRetrieval = true)
        {
            var queue = GetQueue(queueName);
            //List<CloudQueueMessage> messageList = new List<CloudQueueMessage>();
            var messages = await queue.GetMessagesAsync(maxCount);
            //messageList.AddRange(messages);
            var messageObjects = messages.Select(msg => new MessageObject<string> { Message = msg, RetrievedObject = msg.AsString }).ToArray();
            if (deleteOnRetrieval)
            {
                foreach(var msgObj in messageObjects)
                    await queue.DeleteMessageAsync(msgObj.Message);
            }
            return messageObjects;
        }
    }
}