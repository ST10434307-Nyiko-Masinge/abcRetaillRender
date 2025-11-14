using Microsoft.AspNetCore.Mvc;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
{
    public class CustomerController : Controller
    {
        /*
    Code attribution
    https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-8.0
    accessed 7 October 2025
        */
        private readonly FunctionService _functionService;
        private readonly BlobStorageServices _blobStorageServices;



        public CustomerController(FunctionService functionService, BlobStorageServices blobStorageServices)
        {
            _functionService = functionService;
            _blobStorageServices = blobStorageServices;
        }

        /*
    Code attribution
    https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-8.0
    accessed 7 October 2025
        */

        //  customers
        public async Task<IActionResult> Index()
        {
            var customers = await _functionService.GetCustomersAsync();
            return View(customers);
        }


        /*
 Code attribution
 this code was adapted from Microsoft ASP.NET Core MVC CRUD tutorial
 https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-8.0
 accessed 7 October 2025
*/


        //Create customer (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        /*
    Code attribution
    https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-8.0
    accessed 7 October 2025
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, IFormFile photoFile)
        {
            if (ModelState.IsValid)
            {
                // Upload photo if exists
                if (photoFile != null && photoFile.Length > 0)
                {
                    customer.PhotoURL = await _blobStorageServices.UploadImageAsync(photoFile);
                    Console.WriteLine($"Uploaded photo URL: {customer.PhotoURL}");
                }

                customer.RowKey = Guid.NewGuid().ToString();
                customer.PartitionKey = "Customer";

                // Log customer before sending
                Console.WriteLine($"Sending customer to FunctionService: {customer.Name}, {customer.Surname}, {customer.Email}, RowKey: {customer.RowKey}");

                var success = await _functionService.CreateCustomerAsync(customer);

                if (!success)
                {
                    Console.WriteLine("FunctionService.CreateCustomerAsync returned false.");
                    ModelState.AddModelError("", "Failed to create customer via FunctionService.");
                    return View(customer);
                }

                Console.WriteLine("Customer created successfully via FunctionService.");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("ModelState invalid. Customer creation aborted.");
            return View(customer);
        }




        // ➕ Create customer (POST)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Customer customer, IFormFile photoFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (photoFile != null && photoFile.Length > 0)
        //        {
        //            customer.PhotoURL = await _blobStorageServices.UploadImageAsync(photoFile);
        //        }

        //        customer.RowKey = Guid.NewGuid().ToString();
        //        customer.PartitionKey = "Customer";

        //        var success = await _functionService.CreateCustomerAsync(customer);
        //        if (!success)
        //        {
        //            ModelState.AddModelError("", "Failed to create customer via FunctionService.");
        //            return View(customer);
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(customer);
        //}

        //  Edit customer (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var customer = await _functionService.GetCustomerAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        //  Edit customer (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer, IFormFile photoFile)
        {
            if (ModelState.IsValid)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    customer.PhotoURL = await _blobStorageServices.UploadImageAsync(photoFile, customer.PhotoURL);
                }

                var success = await _functionService.UpdateCustomerAsync(customer);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to update customer via FunctionService.");
                    return View(customer);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        //  Delete customer (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var customer = await _functionService.GetCustomerAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        //  Delete customer (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var success = await _functionService.DeleteCustomerAsync(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete customer via FunctionService.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
        /*
    Code attribution
    https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-8.0
    accessed 7 October 2025
        */
        //  Customer details
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var customer = await _functionService.GetCustomerAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }
    }
}



//using Microsoft.AspNetCore.Mvc;
//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services;

//namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
//{
//    public class CustomerController : Controller
//    {
//        private readonly FunctionService _functionService;

//        public CustomerController(FunctionService functionService)
//        {
//            _functionService = functionService;
//        }

//        // 🏠 Show all customers
//        public async Task<IActionResult> Index()
//        {
//            var customers = await _functionService.GetCustomersAsync();
//            return View(customers);
//        }

//        // ➕ Create customer (GET)
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // ➕ Create customer (POST)
//        [HttpPost]
//        public async Task<IActionResult> Create(Customer customer)
//        {
//            if (ModelState.IsValid)
//            {
//                await _functionService.CreateCustomerAsync(customer);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(customer);
//        }

//        // ✏️ Edit customer (GET)
//        [HttpGet]
//        public async Task<IActionResult> Edit(string id)
//        {
//            var customer = await _functionService.GetCustomerAsync(id);
//            if (customer == null) return NotFound();
//            return View(customer);
//        }

//        // ✏️ Edit customer (POST)
//        [HttpPost]
//        public async Task<IActionResult> Edit(Customer customer)
//        {
//            if (ModelState.IsValid)
//            {
//                await _functionService.UpdateCustomerAsync(customer);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(customer);
//        }

//        // ❌ Delete customer (GET)
//        public async Task<IActionResult> Delete(string id)
//        {
//            var customer = await _functionService.GetCustomerAsync(id);
//            if (customer == null) return NotFound();
//            return View(customer);
//        }

//        // ❌ Delete customer (POST)
//        [HttpPost, ActionName("Delete")]
//        public async Task<IActionResult> DeleteConfirmed(string id)
//        {
//            await _functionService.DeleteCustomerAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}



//// CustomerController.cs
//using Microsoft.AspNetCore.Mvc;
//using Azure;
//using Azure.Data.Tables;
//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage;
//using System.Text;
//using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services;

//namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
//{
//    public class CustomerController : Controller
//    {
//        private readonly FunctionService _functionService;
//        private readonly TableStorageServices tableStorageServices;
//        private readonly BlobStorageServices blobStorageServices;
//        private readonly QueueStorageServices queueStorageServices;
//        private readonly FileShareStorageServices fileShareStorageServices;

//        public CustomerController(
//            FunctionService functionService,
//            TableStorageServices tableStorageService,
//            BlobStorageServices blobStorageServices,
//            QueueStorageServices queueStorageServices,
//            FileShareStorageServices fileShareStorageServices)
//        {
//            _functionService = functionService;
//            this.tableStorageServices = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
//            this.blobStorageServices = blobStorageServices ?? throw new ArgumentNullException(nameof(blobStorageServices));
//            this.queueStorageServices = queueStorageServices ?? throw new ArgumentNullException(nameof(queueStorageServices));
//            this.fileShareStorageServices = fileShareStorageServices ?? throw new ArgumentNullException(nameof(fileShareStorageServices));
//        }

//        // READ: Display a list of all customers
//        public async Task<IActionResult> Index()
//        {
//            var customers = await _functionService.GetCustomersAsync();
//            //var customers = await tableStorageServices.GetCustomersAsync();
//            return View(customers);
//        }

//        // CREATE: Display the customer creation form
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // CREATE: Handle the form submission for creating a new customer
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Customer customer, IFormFile photoFile)
//        {
//            if (ModelState.IsValid)
//            {
//                if (photoFile != null && photoFile.Length > 0)
//                {
//                    customer.PhotoURL = await blobStorageServices.UploadImageAsync(photoFile);
//                }

//                customer.RowKey = Guid.NewGuid().ToString();
//                customer.PartitionKey = "Customer";

//                await tableStorageServices.AddCustomerAsync(customer);

//                var message = new
//                {
//                    Action = "New Customer Created",
//                    TimeStamp = DateTime.UtcNow,
//                    Details = new
//                    {
//                        customer.PartitionKey,
//                        customer.RowKey,
//                        customer.Name,
//                        customer.Surname,
//                        customer.Email,
//                    }
//                };
//                await queueStorageServices.SendMessagesAsync(message);

//                return RedirectToAction(nameof(Index));
//            }
//            return View(customer);
//        }

//        // DETAILS: Display the details of a single customer
//        public async Task<IActionResult> Details(string partitionKey, string rowKey)
//        {
//            var customer = await tableStorageServices.GetCustomerAsync(partitionKey, rowKey);
//            if (customer == null)
//            {
//                return NotFound();
//            }
//            return View(customer);
//        }

//        // EDIT: Display the customer edit form
//        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
//        {
//            var customer = await tableStorageServices.GetCustomerAsync(partitionKey, rowKey);
//            if (customer == null)
//            {
//                return NotFound();
//            }
//            return View(customer);
//        }

//        // EDIT: Handle the form submission for updating a customer
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(Customer customer, IFormFile photoFile)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (photoFile != null && photoFile.Length > 0)
//                    {
//                        customer.PhotoURL = await blobStorageServices.UploadImageAsync(photoFile, customer.PhotoURL);
//                    }
//                    else
//                    {
//                        var existingCustomer = await tableStorageServices.GetCustomerAsync(customer.PartitionKey, customer.RowKey);
//                        customer.PhotoURL = existingCustomer?.PhotoURL ?? "/images/default-profile.png";
//                    }

//                    await tableStorageServices.UpdateCustomerAsync(customer);
//                    return RedirectToAction(nameof(Index));
//                }
//                catch (RequestFailedException ex) when (ex.Status == 412)
//                {
//                    ModelState.AddModelError(string.Empty, "The customer you are trying to edit has been modified by another user. Please reload and try again.");
//                }
//            }
//            return View(customer);
//        }

//        // DELETE: Display the customer deletion confirmation page
//        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
//        {
//            var customer = await tableStorageServices.GetCustomerAsync(partitionKey, rowKey);
//            if (customer == null)
//            {
//                return NotFound();
//            }
//            return View(customer);
//        }

//        // DELETE: Handle the form submission for deleting a customer
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
//        {
//            var customer = await tableStorageServices.GetCustomerAsync(partitionKey, rowKey);
//            if (customer != null)
//            {
//                if (!string.IsNullOrEmpty(customer.PhotoURL))
//                {
//                    await blobStorageServices.DeleteImageAsync(customer.PhotoURL);
//                }

//                await tableStorageServices.DeleteCustomerAsync(partitionKey, rowKey);
//            }
//            return RedirectToAction(nameof(Index));
//        }

//        // GET: Customer/Log
//        public async Task<IActionResult> Log()
//        {
//            var logMessages = await queueStorageServices.getMessagesAsync();
//            return View(logMessages);
//        }

//        // GET: Customer/ExportLog
//        [HttpGet]
//        public async Task<IActionResult> ExportLog()
//        {
//            var logMessages = await queueStorageServices.getMessagesAsync();
//            var fileName = $"Log_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

//            using (var stream = new MemoryStream())
//            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
//            {
//                await writer.WriteLineAsync("MessageID,DateTimeOffset,MessageText,InsertionTime");

//                foreach (var log in logMessages)
//                {
//                    await writer.WriteLineAsync(
//                        $"{log.MessageID},{log.DateTimeOffset},{log.MessageText},{log.InsertionTime}");
//                }

//                await writer.FlushAsync();
//                stream.Position = 0;

//                return File(stream.ToArray(), "text/csv", fileName);
//            }
//        }
//    }
//}
