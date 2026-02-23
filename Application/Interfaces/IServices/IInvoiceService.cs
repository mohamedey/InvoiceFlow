using Application.DTOs;

namespace Application.Interfaces.IServices;

public interface IInvoiceService
{
    Task<Guid> CreateAsync(CreateInvoiceDto dto);
    Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
}