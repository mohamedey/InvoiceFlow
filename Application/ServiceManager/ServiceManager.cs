using Application.Interfaces.IServices;
using Application.ServiceManager;

namespace Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly IInvoiceService _invoiceService;

    public ServiceManager(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public IInvoiceService InvoiceService => _invoiceService;
}