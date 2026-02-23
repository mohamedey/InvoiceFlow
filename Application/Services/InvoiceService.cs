using Application.DTOs;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IGenericRepository<Invoice> _invoiceRepo;
    private readonly IGenericRepository<Product> _productRepo;

    public InvoiceService(
        IGenericRepository<Invoice> invoiceRepo,
        IGenericRepository<Product> productRepo)
    {
        _invoiceRepo = invoiceRepo;
        _productRepo = productRepo;
    }

    public async Task<Guid> CreateAsync(CreateInvoiceDto dto)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = Guid.NewGuid().ToString().Substring(0, 8),
            IssueDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Status = "Issued",
            CustomerId = dto.CustomerId,
            Items = new List<InvoiceItem>()
        };

        decimal subTotal = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);

            var lineTotal = product.UnitPrice * item.Quantity;

            invoice.Items.Add(new InvoiceItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.UnitPrice,
                LineTotal = lineTotal
            });

            subTotal += lineTotal;
        }

        invoice.SubTotal = subTotal;
        invoice.TaxAmount = subTotal * 0.15m;
        invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;

        await _invoiceRepo.AddAsync(invoice);
        await _invoiceRepo.SaveAsync();

        return invoice.Id;
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync()
    {
        var invoices = await _invoiceRepo.GetAllAsync();

        return invoices.Select(x => new InvoiceResponseDto
        {
            Id = x.Id,
            InvoiceNumber = x.InvoiceNumber,
            TotalAmount = x.TotalAmount,
            Status = x.Status
        });
    }
}