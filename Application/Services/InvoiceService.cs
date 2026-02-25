using Application.DTOs;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IGenericRepository<Invoice> _invoiceRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<InvoiceItem> _invoiceItemRepo;

    public InvoiceService(
        IGenericRepository<Invoice> invoiceRepo,
        IGenericRepository<Product> productRepo,
        IGenericRepository<InvoiceItem> invoiceItemRepo)
    {
        _invoiceRepo = invoiceRepo;
        _productRepo = productRepo;
        _invoiceItemRepo = invoiceItemRepo;
    }

    public async Task<Guid> CreateAsync(CreateInvoiceDto dto, string userId)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new Exception("Invoice must contain at least one item");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = Guid.NewGuid().ToString("N")[..8],
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

    public async Task<string> UpdateAsync(Guid id, CreateInvoiceDto dto, string userId)
    {
        var invoice = await _invoiceRepo.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (invoice.UserId != userId)
            throw new UnauthorizedAccessException();

        invoice.DueDate = dto.DueDate;
        invoice.Status = "Updated";

        await _invoiceRepo.SaveAsync();

        var oldItems = _invoiceItemRepo.Query()
            .Where(x => x.InvoiceId == id)
            .ToList();

        _invoiceItemRepo.RemoveRange(oldItems);

        await _invoiceRepo.SaveAsync();

        decimal subTotal = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);

            var lineTotal = product.UnitPrice * item.Quantity;

            await _invoiceItemRepo.AddAsync(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = id,
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

        await _invoiceRepo.SaveAsync();

        return "Invoice updated successfully";
    }
    public async Task<string> DeleteAsync(Guid id, string userId, bool isAdmin)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (!isAdmin && invoice.UserId != userId)
            throw new UnauthorizedAccessException();

        await _invoiceRepo.DeleteAsync(invoice);
        await _invoiceRepo.SaveAsync();

        return "Invoice deleted successfully";
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

    public async Task<InvoiceDetailsDto> GetDetailsAsync(Guid id, string userId, bool isAdmin)
    {
        var invoice = await _invoiceRepo.Query()
            .Include(x => x.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (!isAdmin && invoice.UserId != userId)
            throw new UnauthorizedAccessException();

        return new InvoiceDetailsDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            Items = invoice.Items.Select(i => new InvoiceItemDetailsDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }

    public async Task<PagedResult<InvoiceResponseDto>> GetPagedByUserAsync(
     string userId,
     bool isAdmin,
     int page,
     int pageSize)
    {
        var query = _invoiceRepo.Query();

        if (!isAdmin)
            query = query.Where(x => x.UserId == userId);

        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InvoiceResponseDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                TotalAmount = x.TotalAmount,
                Status = x.Status
            })
            .ToListAsync();

        return new PagedResult<InvoiceResponseDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Data = data
        };
    }
}