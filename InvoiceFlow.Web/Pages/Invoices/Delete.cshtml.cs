using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public InvoiceResponseDto Invoice { get; set; }

    public DeleteModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var client = _factory.CreateClient("ApiClient");
        Invoice = await client.GetFromJsonAsync<InvoiceResponseDto>($"invoice/{id}");

        if (Invoice == null)
            return RedirectToPage("/Invoices/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var client = _factory.CreateClient("ApiClient");

        var response = await client.DeleteAsync($"invoice/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Failed to delete invoice.";
            return RedirectToPage("/Invoices/Index");
        }

        TempData["Success"] = "Invoice deleted successfully.";
        return RedirectToPage("/Invoices/Index");
    }
}