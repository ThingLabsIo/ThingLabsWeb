using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Azure;
using Microsoft.Azure.Devices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ThingLabsWeb.Models;

namespace ThingLabsWeb.Controllers
{
    [Authorize]
    public class MessageController : ApiController
    {
        // GET api/<controller>
        public async Task<IEnumerable<WeatherRecord>> Get()
        {
            List<string> deviceIds = await this.GetUserDeviceIds();

            return GetRecordsForDevices(deviceIds);
        }

        // GET api/<controller>/5
        public async Task<IEnumerable<WeatherRecord>> Get(string id)
        {
            List<string> deviceIds = new List<string>();
            deviceIds.Add(id);

            return GetRecordsForDevices(deviceIds);
        }

        private IEnumerable<WeatherRecord> GetRecordsForDevices(List<string> deviceIds)
        {
            string storageAccountConnectionString = CloudConfigurationManager.GetSetting("WeatherTableStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("weather");

            string filter = "";
            foreach (string id in deviceIds)
            {
                string newFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);
                if (filter == "")
                {
                    filter = newFilter;
                }
                else
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.Or, newFilter);
                }
            }

            TableQuery<WeatherRecord> rangeQuery = new TableQuery<WeatherRecord>().Where(filter).Take(100);

            return table.ExecuteQuery(rangeQuery);
        }

        //TODO - centralize this method
        private string GetIoTHubConnectionString()
        {
            var userId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string connectionString = currentUser.IoTHubConnectionString;
            return connectionString;
        }

        private async Task<IEnumerable<Device>> GetDevices()
        {
            string connectionString = GetIoTHubConnectionString();

            RegistryManager registryManager;
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            IEnumerable<Device> devices;
            try
            {
                devices = await registryManager.GetDevicesAsync(1000000);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return devices;
        }

        private async Task<List<string>> GetUserDeviceIds()
        {
            IEnumerable<Device> devices = await GetDevices();

            List<string> deviceIds = new List<string>();
            foreach (Device d in devices)
            {
                deviceIds.Add(d.Id);
            }

            return deviceIds;
        }
    }
}
