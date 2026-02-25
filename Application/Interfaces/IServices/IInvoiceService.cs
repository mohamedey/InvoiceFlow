using Application.DTOs;

namespace Application.Interfaces.IServices;

public interface IInvoiceService
{
    Task<Guid> CreateAsync(CreateInvoiceDto dto, string userId);
    Task<string> UpdateAsync(Guid id, CreateInvoiceDto dto, string userId);
    Task<string> DeleteAsync(Guid id, string userId, bool isAdmin);
    Task<InvoiceResponseDto> GetByIdAsync(Guid id, string userId, bool isAdmin);
    Task<InvoiceDetailsDto> GetDetailsAsync(Guid id, string userId, bool isAdmin);
    Task<PagedResult<InvoiceResponseDto>> GetPagedByUserAsync( string userId,bool isAdmin,int page,int pageSize);
}