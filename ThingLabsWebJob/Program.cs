using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;
using System.Configuration;
using System.Threading;
//using Microsoft.Azure.Devices;

namespace ThingLabsWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            Run();

            host.RunAndBlock();
        }

        static async void Run()
        {
            List<HubReference> hubs = new List<HubReference>();

            while (true)
            {
                Console.WriteLine("Receive messages\n");

                string watcherTableConnectionString = ConfigurationManager.ConnectionStrings["WatcherTableStorage"].ToString();
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(watcherTableConnectionString);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("watcher");

                TableQuery<WatcherRecordModel> rangeQuery = new TableQuery<WatcherRecordModel>().Take(10000);

                IEnumerable<WatcherRecordModel> records = table.ExecuteQuery(rangeQuery);

                foreach (WatcherRecordModel record in records)
                {
                    if (record.Timeout < DateTime.Now)
                    {
                        TableOperation deleteOperation = TableOperation.Delete(record);
                        table.Execute(deleteOperation);
                    }
                    else if (!IsHubInList(record, hubs))
                    {
                        HubReference hub = new HubReference(record.IoTHubConnectionString, record.IotHubEndpoint);
                        hub.GetMessagesFromHub();
                    }
                }

                Thread.Sleep(60 * 1000);
            }
        }

        private static bool IsHubInList(WatcherRecordModel record, List<HubReference> hubs)
        {
            foreach (HubReference hub in hubs)
            {
                if (hub.IoTHubConnectionString == record.IoTHubConnectionString)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
