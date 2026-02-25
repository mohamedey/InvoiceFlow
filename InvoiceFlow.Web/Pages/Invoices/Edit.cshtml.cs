using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

[Authorize]
public class EditModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    [BindProperty]
    public CreateInvoiceRequest Invoice { get; set; } = new();

    public List<ProductDto> Products { get; set; } = new();

    public Guid InvoiceId { get; set; }

    public EditModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        InvoiceId = id;

        var client = _factory.CreateClient("ApiClient");

        var invoice = await client.GetFromJsonAsync<InvoiceDetailsDto>($"invoice/{id}");
        Products = await client.GetFromJsonAsync<List<ProductDto>>("product") ?? new();

        if (invoice == null)
            return RedirectToPage("/Invoices/Index");

        Invoice.CustomerId = invoice.CustomerId;
        Invoice.DueDate = invoice.DueDate;

        foreach (var item in invoice.Items)
        {
            Invoice.Items.Add(new CreateInvoiceItemRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!ModelState.IsValid)
            return Page();

        var client = _factory.CreateClient("ApiClient");

        var response = await client.PutAsJsonAsync($"invoice/{id}", Invoice);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Error updating invoice");
            return Page();
        }

        return RedirectToPage("/Invoices/Index");
    }
}

public class InvoiceDetailsDto
{
    public Guid CustomerId { get; set; }
    public DateTime DueDate { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public class InvoiceItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}