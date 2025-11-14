using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProductStorageService _productService;
        private readonly OrderStorageService _orderService;
        private readonly TableStorageServices _customerService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ProductStorageService productService,
            OrderStorageService orderService,
            TableStorageServices customerService,
            ILogger<AdminController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            var orders = await _orderService.GetOrdersAsync();
            var customers = await _customerService.GetCustomersAsync();

            var dashboard = new AdminDashboardViewModel
            {
                TotalProducts = products.Count,
                ActiveProducts = products.Count(p => p.IsActive),
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == "Pending"),
                TotalCustomers = customers.Count,
                RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).ToList(),
                TotalRevenue = orders.Where(o => o.Status != "Cancelled").Sum(o => o.TotalAmount)
            };

            return View(dashboard);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return View(orders.OrderByDescending(o => o.OrderDate).ToList());
        }

        public async Task<IActionResult> OrderDetails(string id)
        {
            var order = await _orderService.GetOrderAsync("Order", id);
            if (order == null)
            {
                return NotFound();
            }

            var orderItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(order.OrderItemsJson ?? "[]");
            ViewBag.OrderItems = orderItems ?? new List<CartItem>();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(string id, string status)
        {
            var order = await _orderService.GetOrderAsync("Order", id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _orderService.UpdateOrderAsync(order);

            return RedirectToAction("OrderDetails", new { id });
        }

        public async Task<IActionResult> Customers()
        {
            var customers = await _customerService.GetCustomersAsync();
            return View(customers);
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}

