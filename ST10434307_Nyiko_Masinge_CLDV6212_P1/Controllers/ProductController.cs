using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductStorageService _productService;
        private readonly BlobStorageServices _blobService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ProductStorageService productService,
            BlobStorageServices blobService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ImageURL = await _blobService.UploadImageAsync(imageFile);
                }

                product.PartitionKey = "Product";
                product.RowKey = Guid.NewGuid().ToString();
                product.IsActive = true;

                await _productService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public async Task<IActionResult> Edit(string id)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(product.ImageURL))
                        {
                            await _blobService.DeleteImageAsync(product.ImageURL);
                        }
                        product.ImageURL = await _blobService.UploadImageAsync(imageFile);
                    }

                    await _productService.UpdateProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                }
            }

            return View(product);
        }

        public async Task<IActionResult> Delete(string id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _productService.GetProductAsync("Product", id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageURL))
                {
                    await _blobService.DeleteImageAsync(product.ImageURL);
                }

                await _productService.DeleteProductAsync("Product", id);
            }

            return RedirectToAction(nameof(Index));
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
    }
}

