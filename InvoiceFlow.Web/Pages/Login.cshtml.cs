using InvoiceFlow.Web.Models;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace InvoiceFlow.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthApiService _authService;

        [BindProperty]
        public LoginRequest Login { get; set; }

        public LoginModel(AuthApiService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var result = await _authService.LoginAsync(Login);

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, result.Email),
            new Claim(ClaimTypes.Role, result.Role),
            new Claim("JWT", result.Token)
        };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToPage("/Dashboard");
            }
            catch
            {
                ModelState.AddModelError("", "Invalid login");
                return Page();
            }
        }
    }
}
