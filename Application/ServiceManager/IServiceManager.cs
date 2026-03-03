using Application.Interfaces.IServices;

namespace Application.ServiceManager;

public interface IServiceManager
{
    IInvoiceService InvoiceService { get; }
    IProductService ProductService { get; }

}