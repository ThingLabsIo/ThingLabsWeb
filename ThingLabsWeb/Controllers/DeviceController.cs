using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ThingLabsWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ThingLabsWeb.Controllers
{
    [Authorize]
    public class DeviceController : Controller
    {
        // GET: Device
        public async Task<ActionResult> Index()
        {
            IEnumerable<Device> devices = await GetDevices();

            ViewBag.Devices = devices;

            var userId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ViewBag.UserId = manager.FindById(User.Identity.GetUserId());

            return View();
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

        //TODO - centralize this method
        private string GetIoTHubConnectionString()
        {
            var userId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            string connectionString = currentUser.IoTHubConnectionString;
            return connectionString;
        }

        // GET: Device/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                string connectionString = GetIoTHubConnectionString();
                string newId = await GetDeviceAsync(id, connectionString, ViewBag);

                var userId = User.Identity.GetUserId();
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                ViewBag.UserId = manager.FindById(User.Identity.GetUserId());
                return View();
            }
            catch (Exception ex)
            {
                ex = ex;
                return View();
            }
        }
        private async static Task<bool> GetDeviceAsync(string deviceId, string connectionString, dynamic viewBag)
        {
            RegistryManager registryManager;
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            Device device;
            try
            {
                device = await registryManager.GetDeviceAsync(deviceId);

                viewBag.device = device;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Messages()
        {
            return View();
        }

        private async static Task<string> AddDeviceAsync(string deviceId, string connectionString)
        {
            RegistryManager registryManager;
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            string deviceKey = device.Authentication.SymmetricKey.PrimaryKey;

            return deviceKey;
        }

        // POST: Device/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                string connectionString = GetIoTHubConnectionString();
                //TODO: Add DeviceId and Connnection String to the form collection
                string deviceId = collection["DeviceId"];
                string newId = await AddDeviceAsync(deviceId, connectionString);
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Device/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Device/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Device/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Device/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
