using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingLabsWeb
{
    public class WeatherRecord : TableEntity
    {
        public WeatherRecord()
        {
            this.RowKey = Guid.NewGuid().ToString();
        }

        public double fahrenheit { get; set; }
        public double celsius { get; set; }
        public double relativeHumidity { get; set; }
        public string location { get; set; }
        public double pressure { get; set; }
        public double meters { get; set; }
        private string _deviceId;
        public string deviceId
        {
            get { return _deviceId; }
            set
            {
                _deviceId = value;
                this.PartitionKey = deviceId;
            }
        }
    }
}
