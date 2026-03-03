using Application.Interfaces.IServices;
using InvoiceFlow.Web.Models;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Domain.Entities;

namespace InvoiceFlow.Web.Pages.Products
{
    public class AddProductModel : PageModel
    {
        private readonly ProductApiService _productApiService;

        public AddProductModel(ProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        [BindProperty]
        public ProductViewModel Product { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = Product.Name,
                Description = Product.Description,
                UnitPrice = Product.UnitPrice
            };

            var result = await _productApiService.AddProductAsync(product);
            if (result)
            {
                TempData["Message"] = "Product added successfully";
                return RedirectToPage("OurProducts");
            }
            else
            {
                TempData["Message"] = "Failed to add product";
                return Page();
            }
        }
    }
}