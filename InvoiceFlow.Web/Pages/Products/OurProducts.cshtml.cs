using InvoiceFlow.Web.Models;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceFlow.Web.Pages.Products
{
    public class OurProductsModel : PageModel
    {
        private readonly ProductApiService _productApiService;

        public List<ProductViewModel> Products { get; set; } = new();

        public OurProductsModel(ProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        public async Task OnGetAsync()
        {
            Products = await _productApiService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var response = await _productApiService.DeleteAsync(id);
            if (response.Contains("successfully"))
            {
                TempData["Message"] = "Product deleted successfully";
            }
            else
            {
                TempData["Message"] = "Failed to delete product";
            }

            return RedirectToPage();
        }
    }
}