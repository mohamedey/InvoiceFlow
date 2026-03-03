using Application.Interfaces.IServices;
using Application.ServiceManager;

namespace Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly IInvoiceService _invoiceService;
    private readonly IProductService _productService;

    public ServiceManager(IInvoiceService invoiceService ,IProductService productService)
    {
        _invoiceService = invoiceService;
        _productService = productService;
    }

    public IInvoiceService InvoiceService => _invoiceService;
    public IProductService ProductService => _productService;

}