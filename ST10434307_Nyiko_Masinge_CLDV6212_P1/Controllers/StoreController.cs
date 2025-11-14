using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
{
    public class StoreController : Controller
    {
        private readonly ProductStorageService _productService;
        private readonly OrderStorageService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StoreController> _logger;

        public StoreController(
            ProductStorageService productService,
            OrderStorageService orderService,
            UserManager<ApplicationUser> userManager,
            ILogger<StoreController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? category = null, string? search = null)
        {
            var products = await _productService.GetActiveProductsAsync();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.Categories = (await _productService.GetActiveProductsAsync())
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _productService.GetProductAsync("Product", id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(string productId, int quantity = 1)
        {
            var cart = GetCart();
            var product = _productService.GetProductAsync("Product", productId).Result;

            if (product == null)
            {
                return NotFound();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.RowKey,
                    ProductName = product.Name,
                    ProductImage = product.ImageURL,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            return RedirectToAction("Cart");
        }

        [Authorize]
        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateCart(string productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(cart);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveFromCart(string productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Cart");
        }

        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Items.Count == 0)
            {
                return RedirectToAction("Cart");
            }

            var user = _userManager.GetUserAsync(User).Result;
            var model = new CheckoutViewModel
            {
                Cart = cart,
                ShippingAddress = user?.Address ?? "",
                ShippingCity = user?.City ?? "",
                ShippingPostalCode = user?.PostalCode ?? "",
                ShippingCountry = user?.Country ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (cart.Items.Count == 0)
            {
                return RedirectToAction("Cart");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var order = new Order
                {
                    PartitionKey = "Order",
                    RowKey = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    UserEmail = user.Email ?? "",
                    ShippingAddress = model.ShippingAddress,
                    ShippingCity = model.ShippingCity,
                    ShippingPostalCode = model.ShippingPostalCode,
                    ShippingCountry = model.ShippingCountry,
                    TotalAmount = cart.Total,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    OrderItemsJson = JsonSerializer.Serialize(cart.Items)
                };

                await _orderService.AddOrderAsync(order);

                // Clear cart
                ClearCart();

                return RedirectToAction("OrderConfirmation", new { id = order.RowKey });
            }

            model.Cart = cart;
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(string id)
        {
            var order = await _orderService.GetOrderAsync("Order", id);
            if (order == null || order.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders.OrderByDescending(o => o.OrderDate).ToList());
        }

        [Authorize]
        public async Task<IActionResult> OrderDetails(string id)
        {
            var order = await _orderService.GetOrderAsync("Order", id);
            if (order == null || order.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var orderItems = JsonSerializer.Deserialize<List<CartItem>>(order.OrderItemsJson ?? "[]");
            ViewBag.OrderItems = orderItems ?? new List<CartItem>();

            return View(order);
        }

        private ShoppingCart GetCart()
        {
            var cartJson = HttpContext.Session.GetString("ShoppingCart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new ShoppingCart();
            }

            return JsonSerializer.Deserialize<ShoppingCart>(cartJson) ?? new ShoppingCart();
        }

        private void SaveCart(ShoppingCart cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("ShoppingCart", cartJson);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("ShoppingCart");
        }
    }

    public class CheckoutViewModel
    {
        public ShoppingCart Cart { get; set; } = new ShoppingCart();

        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [Display(Name = "City")]
        public string ShippingCity { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Postal Code")]
        public string ShippingPostalCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Country")]
        public string ShippingCountry { get; set; } = string.Empty;
    }
}

