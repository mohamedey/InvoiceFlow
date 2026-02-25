using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    [BindProperty]
    public CreateInvoiceRequest Invoice { get; set; } = new();

    public List<ProductDto> Products { get; set; } = new();

    public CreateModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("ApiClient");
        Products = await client.GetFromJsonAsync<List<ProductDto>>("product") ?? new();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var client = _factory.CreateClient("ApiClient");

        var response = await client.PostAsJsonAsync("invoice", Invoice);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Error creating invoice");
            return Page();
        }

        return RedirectToPage("/Invoices/Index");
    }
}

public class CreateInvoiceRequest
{
    public Guid CustomerId { get; set; }
    public DateTime DueDate { get; set; }
    public List<CreateInvoiceItemRequest> Items { get; set; } = new();
}

public class CreateInvoiceItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}