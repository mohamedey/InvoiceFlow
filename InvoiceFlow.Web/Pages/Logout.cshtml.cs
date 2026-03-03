using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InvoiceFlow.Web.Pages;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToPage("/Login");
    }
}