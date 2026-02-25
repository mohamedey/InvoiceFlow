using Microsoft.AspNetCore.Authorization;
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

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("ApiClient");
        Invoices = await client.GetFromJsonAsync<List<InvoiceResponseDto>>("invoice/my") ?? new();
    }
}

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}