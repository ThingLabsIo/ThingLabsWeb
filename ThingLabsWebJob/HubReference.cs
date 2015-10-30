using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Configuration;

namespace ThingLabsWebJob
{
    class HubReference
    {
        public HubReference(string connectionStringName, string iotHubD2cEndpointName)
        {
            InitializeTable();

            IoTHubConnectionString = connectionStringName;
            iotHubD2cEndpoint = iotHubD2cEndpointName;
        }
        public string IoTHubConnectionString { get; internal set; }

        string iotHubD2cEndpoint = "";
        EventHubClient eventHubClient;

        public bool GetMessagesFromHub()
        {
            eventHubClient = EventHubClient.CreateFromConnectionString(IoTHubConnectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }

            return true;
        }

        private async Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));

                WeatherRecord record = JsonConvert.DeserializeObject< WeatherRecord>(data);

                try
                {
                    WriteToTable(record);
                }
                catch (Exception ex)
                {
                    ex = ex;
                }
            }
        }

        private void WriteToTable(WeatherRecord record)
        {
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(record);

            // Execute the insert operation.
            _weatherTable.Execute(insertOperation);
        }

        CloudTable _weatherTable;

        private void InitializeTable()
        {
            // Retrieve the storage account from the connection string.

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            _weatherTable = tableClient.GetTableReference("weather");
            _weatherTable.CreateIfNotExists();
        }
    }
}
