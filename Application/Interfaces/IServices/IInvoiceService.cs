using Application.DTOs;

namespace Application.Interfaces.IServices;

public interface IInvoiceService
{
    Task<Guid> CreateAsync(CreateInvoiceDto dto, string userId);
    Task UpdateAsync(Guid id, CreateInvoiceDto dto, string userId);
    Task DeleteAsync(Guid id, string userId, bool isAdmin);
    Task<InvoiceResponseDto> GetByIdAsync(Guid id, string userId, bool isAdmin);
    Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
    Task<IEnumerable<InvoiceResponseDto>> GetByUserAsync(string userId);
}