using Application.Interfaces.IServices;
using Domain.Entities;
using InvoiceFlow.Web.Models;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace InvoiceFlow.Web.Pages.Products
{
    public class EditProductModel : PageModel
    {
        private readonly ProductApiService _productApiService;

        public EditProductModel(ProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        [BindProperty]
        public ProductViewModel Product { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var product = await _productApiService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["Message"] = "Product not found.";
                return NotFound();
            }

            // Prepare the product data for editing
            Product = new ProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please provide valid data.";
                return Page();
            }

            // Create the product object with the new values
            var productToUpdate = new Product
            {
                Id = id,
                Name = Product.Name,
                Description = Product.Description,
                UnitPrice = Product.UnitPrice
            };

            var result = await _productApiService.UpdateProductAsync(productToUpdate);
            if (result)
            {
                TempData["Message"] = "Product updated successfully!";
                return RedirectToPage("OurProducts");
            }

            TempData["Message"] = "Failed to update product.";
            return Page();
        }
    }
}