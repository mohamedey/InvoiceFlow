using Application.DTOs;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InvoiceFlow.Web.Pages.Dashboard;

public class DashboardModel : PageModel
{
    private readonly DashboardApiService _dashboardService;

    public DashboardModel(DashboardApiService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public decimal TotalRevenue { get; set; }
    public int TotalInvoices { get; set; }

    public List<InvoiceResponseDto> Invoices { get; set; } = new();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public async Task OnGetAsync(int page = 1)
    {
        int pageSize = 5;

        var result = await _dashboardService.GetMyInvoicesAsync(page, pageSize);

        Invoices = result.Data;
        TotalInvoices = result.TotalCount;
        TotalRevenue = result.Data.Sum(x => x.TotalAmount);

        CurrentPage = page;
        TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
    }
}