using Application.DTOs;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services;

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

    public async Task<Guid> CreateAsync(CreateInvoiceDto dto, string userId)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new Exception("Invoice must contain at least one item");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = Guid.NewGuid().ToString().Substring(0, 8),
            IssueDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Status = "Issued",
            CustomerId = dto.CustomerId,
            UserId = userId,
            Items = new List<InvoiceItem>()
        };

        decimal subTotal = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            var lineTotal = product.UnitPrice * item.Quantity;

            invoice.Items.Add(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
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

    public async Task UpdateAsync(Guid id, CreateInvoiceDto dto, string userId)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (invoice.UserId != userId)
            throw new UnauthorizedAccessException();

        if (dto.Items == null || !dto.Items.Any())
            throw new Exception("Invoice must contain at least one item");

        invoice.DueDate = dto.DueDate;
        invoice.Status = "Updated";
        invoice.Items.Clear();

        decimal subTotal = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            var lineTotal = product.UnitPrice * item.Quantity;

            invoice.Items.Add(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
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

        await _invoiceRepo.UpdateAsync(invoice);
        await _invoiceRepo.SaveAsync();
    }

    public async Task DeleteAsync(Guid id, string userId, bool isAdmin)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (!isAdmin && invoice.UserId != userId)
            throw new UnauthorizedAccessException();

        await _invoiceRepo.DeleteAsync(invoice);
        await _invoiceRepo.SaveAsync();
    }

    public async Task<InvoiceResponseDto> GetByIdAsync(Guid id, string userId, bool isAdmin)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (!isAdmin && invoice.UserId != userId)
            throw new UnauthorizedAccessException();

        return new InvoiceResponseDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status
        };
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

    public async Task<IEnumerable<InvoiceResponseDto>> GetByUserAsync(string userId)
    {
        var invoices = await _invoiceRepo.GetAllAsync();

        return invoices
            .Where(x => x.UserId == userId)
            .Select(x => new InvoiceResponseDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                TotalAmount = x.TotalAmount,
                Status = x.Status
            });
    }
}