using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingLabsWebJob
{
    public class WatcherRecordModel : TableEntity
    {
        public WatcherRecordModel() { }
        public DateTime Timeout { get; set; }
        public string IoTHubConnectionString { get; set; }
        public string IotHubEndpoint { get; set; }
    }
}