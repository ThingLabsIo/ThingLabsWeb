using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThingLabsWeb.Models
{
    public class WatcherRecordModel : TableEntity
    {
        public WatcherRecordModel(string Id, string IoTHubConnectionString, string IoTHubEndpoint)
        {
            PartitionKey = Id;
            RowKey = Guid.NewGuid().ToString();
            this.IoTHubConnectionString = IoTHubConnectionString;
            this.IotHubEndpoint = IoTHubEndpoint;
        }
        public WatcherRecordModel() { }
        public DateTime Timeout { get; set; }
        public string IoTHubConnectionString { get; set; }
        public string IotHubEndpoint { get; set; }
    }
}