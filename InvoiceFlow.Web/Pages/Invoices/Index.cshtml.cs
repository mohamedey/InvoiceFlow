using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public List<InvoiceResponseDto> Invoices { get; set; } = new();

    public IndexModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task OnGetAsync(int page = 1)
    {
        var client = _factory.CreateClient("ApiClient");

        var result = await client.GetFromJsonAsync<PagedResult<InvoiceResponseDto>>
            ($"invoice/my?page={page}&pageSize=10");

        Invoices = result?.Data ?? new();
    }
    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var client = _factory.CreateClient("ApiClient");

        var response = await client.DeleteAsync($"invoice/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Delete failed";
            return RedirectToPage();
        }

        TempData["Success"] = "Invoice deleted successfully";
        return RedirectToPage();
    }
}

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}