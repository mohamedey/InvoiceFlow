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
    public List<CustomerDto> Customers { get; set; } = new();

    public CreateModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task OnGetAsync()
    {
        Invoice.DueDate = DateTime.Today;

        var client = _factory.CreateClient("ApiClient");

        Products = await client.GetFromJsonAsync<List<ProductDto>>("product") ?? new();
        Customers = await client.GetFromJsonAsync<List<CustomerDto>>("customer") ?? new();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _factory.CreateClient("ApiClient");

        try
        {
            if (!ModelState.IsValid)
            {
                Products = await client.GetFromJsonAsync<List<ProductDto>>("product") ?? new();
                Customers = await client.GetFromJsonAsync<List<CustomerDto>>("customer") ?? new();
                return Page();
            }

            var response = await client.PostAsJsonAsync("invoice", Invoice);

            if (!response.IsSuccessStatusCode)
            {
                Products = await client.GetFromJsonAsync<List<ProductDto>>("product") ?? new();
                Customers = await client.GetFromJsonAsync<List<CustomerDto>>("customer") ?? new();
                TempData["Error"] = "Error creating invoice";
                return Page();
            }

            TempData["Success"] = "Invoice created successfully";
            return RedirectToPage("/Invoices/Index");
        }
        catch
        {
            Products = await client.GetFromJsonAsync<List<ProductDto>>("product") ?? new();
            Customers = await client.GetFromJsonAsync<List<CustomerDto>>("customer") ?? new();
            TempData["Error"] = "Error creating invoice";
            return Page();
        }
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
    public string? Name { get; set; }
}

public class CustomerDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}