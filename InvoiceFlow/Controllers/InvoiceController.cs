using Application.DTOs;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using System.Security.Claims;

namespace InvoiceFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Create(CreateInvoiceDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var id = await _invoiceService.CreateAsync(dto, userId);
        return Ok(id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Update(Guid id, CreateInvoiceDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var message = await _invoiceService.UpdateAsync(id, dto, userId);

        return Ok(new { message });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var message = await _invoiceService.DeleteAsync(id, userId, isAdmin);

        return Ok(new { message });
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var data = await _invoiceService.GetDetailsAsync(id, userId, isAdmin);
        return Ok(data);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy(int page = 1, int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var data = await _invoiceService.GetPagedByUserAsync(
            userId,
            isAdmin,
            page,
            pageSize);

        return Ok(data);
    }

    // Admin can download any invoice, users can only download their own invoices
    [HttpGet("{id}/pdf")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var invoice = await _invoiceService.GetDetailsAsync(id, userId, isAdmin);
        var logoPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Assets",
            "logo.png"
        );

        var logoBytes = System.IO.File.Exists(logoPath)
            ? System.IO.File.ReadAllBytes(logoPath)
            : null;

        var companyName = "InvoiceFlow Solutions";
        var companyTagline = "Professional Invoicing Platform";


        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11));

                // ================= HEADER =================
                page.Header().Column(header =>
                {
                    if (logoBytes != null)
                    {
                        header.Item()
                            .AlignCenter()
                            .Height(50)
                            .Image(logoBytes);

                        header.Item().PaddingBottom(8);
                    }

                    header.Item().AlignCenter()
                        .Text(companyName)
                        .FontSize(18)
                        .Bold();

                    header.Item().AlignCenter()
                        .Text(companyTagline)
                        .FontSize(9)
                        .FontColor("#777777");

                    header.Item().PaddingVertical(12);

                    header.Item()
                        .LineHorizontal(1)
                        .LineColor("#dddddd");
                });

                // ================= CONTENT =================
                page.Content().PaddingVertical(25).Column(content =>
                {
                    // ===== INFO SECTION =====
                    content.Item().Row(row =>
                    {
                        row.RelativeColumn().Column(left =>
                        {
                            left.Item().Text("BILL TO")
                                .Bold()
                                .FontSize(12);

                            left.Item().PaddingTop(5);
                            left.Item().Text(invoice.CustomerName)
                                .FontSize(13);
                        });

                        row.ConstantColumn(260).Column(right =>
                        {
                            right.Item().AlignRight()
                                .Text($"Invoice #: {invoice.InvoiceNumber}")
                                .Bold();

                            right.Item().AlignRight()
                                .Text($"Due Date: {invoice.DueDate:dd MMM yyyy}");

                            right.Item().AlignRight()
                                .Text($"Status: {invoice.Status}");
                        });
                    });

                    content.Item().PaddingVertical(30);

                    // ================= TABLE =================
                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell()
                                .PaddingBottom(6)
                                .BorderBottom(1)
                                .BorderColor("#cccccc")
                                .Text("Product")
                                .Bold();

                            header.Cell()
                                .PaddingBottom(6)
                                .BorderBottom(1)
                                .BorderColor("#cccccc")
                                .AlignCenter()
                                .Text("Qty")
                                .Bold();

                            header.Cell()
                                .PaddingBottom(6)
                                .BorderBottom(1)
                                .BorderColor("#cccccc")
                                .AlignRight()
                                .Text("Unit Price")
                                .Bold();

                            header.Cell()
                                .PaddingBottom(6)
                                .BorderBottom(1)
                                .BorderColor("#cccccc")
                                .AlignRight()
                                .Text("Line Total")
                                .Bold();
                        });

                        foreach (var item in invoice.Items)
                        {
                            table.Cell()
                                .PaddingVertical(8)
                                .Text(item.ProductName);

                            table.Cell()
                                .PaddingVertical(8)
                                .AlignCenter()
                                .Text(item.Quantity.ToString());

                            table.Cell()
                                .PaddingVertical(8)
                                .AlignRight()
                                .Text(item.UnitPrice.ToString("C"));

                            table.Cell()
                                .PaddingVertical(8)
                                .AlignRight()
                                .Text((item.UnitPrice * item.Quantity).ToString("C"));
                        }
                    });

                    content.Item().PaddingTop(35);

                    // ================= TOTAL SECTION =================
                    content.Item()
                        .AlignRight()
                        .Width(300)
                        .Column(total =>
                        {
                            total.Item().Row(r =>
                            {
                                r.RelativeColumn().Text("Subtotal");
                                r.ConstantColumn(120)
                                    .AlignRight()
                                    .Text(invoice.TotalAmount.ToString("C"));
                            });

                            total.Item()
                                .PaddingVertical(10)
                                .LineHorizontal(1)
                                .LineColor("#cccccc");

                            total.Item().Row(r =>
                            {
                                r.RelativeColumn()
                                    .Text("TOTAL")
                                    .Bold()
                                    .FontSize(16);

                                r.ConstantColumn(120)
                                    .AlignRight()
                                    .Text(invoice.TotalAmount.ToString("C"))
                                    .Bold()
                                    .FontSize(16);
                            });
                        });
                });

                // ================= FOOTER =================
                page.Footer()
                    .AlignCenter()
                    .Text("@ Developed by Mohammed Badawi")
                    .FontSize(9)
                    .FontColor("#888888");
            });
        });
        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"Invoice-{invoice.InvoiceNumber}.pdf");
    }
}