using Application.DTOs;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InvoiceFlow.Web.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly InvoiceApiService _service;

        public List<InvoiceResponseDto> Invoices { get; set; }

        public DashboardModel(InvoiceApiService service)
        {
            _service = service;
        }

        public async Task OnGetAsync()
        {
            Invoices = await _service.GetMyAsync();
        }
    }
}
