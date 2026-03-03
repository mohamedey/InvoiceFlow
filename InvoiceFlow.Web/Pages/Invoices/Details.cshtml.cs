using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public Application.DTOs.InvoiceDetailsDto? Invoice { get; set; }
    public DetailsModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    public async Task<IActionResult> OnGetDownloadAsync(Guid id)
    {
        var client = _factory.CreateClient("ApiClient");

        var response = await client.GetAsync($"invoice/{id}/pdf");

        if (!response.IsSuccessStatusCode)
            return RedirectToPage();

        var bytes = await response.Content.ReadAsByteArrayAsync();

        return File(bytes, "application/pdf", $"Invoice-{id}.pdf");
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var client = _factory.CreateClient("ApiClient");

        var response = await client.GetAsync($"invoice/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Invoices/Index");

        Invoice = await response.Content.ReadFromJsonAsync<Application.DTOs.InvoiceDetailsDto>();

        return Page();
    }
}
