using InvoiceFlow.Web.Models;
using InvoiceFlow.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InvoiceFlow.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AuthApiService _authService;
        private readonly CustomerApiService _customerService;

        public IndexModel(AuthApiService authService,
                          CustomerApiService customerService)
        {
            _authService = authService;
            _customerService = customerService;
        }

        public List<UserRoleViewModel> Users { get; set; }
        public List<CustomerViewModel> Customers { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public CustomerViewModel NewCustomer { get; set; }

        public string Message { get; set; }
        public string RoleMessage { get; set; }
        public string CustomerMessage { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _authService.GetAllUsersAsync();
            Customers = await _customerService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var request = new RegisterRequest
            {
                Email = Email,
                Password = Password,
                FullName = FullName
            };

            Message = await _authService.AdminCreateUserAsync(request);

            Users = await _authService.GetAllUsersAsync();
            Customers = await _customerService.GetAllAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateRoleAsync(string userId, string role)
        {
            RoleMessage = await _authService.UpdateRoleAsync(userId, role);

            Users = await _authService.GetAllUsersAsync();
            Customers = await _customerService.GetAllAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateCustomerAsync()
        {
            CustomerMessage = await _customerService.CreateAsync(NewCustomer);

            Users = await _authService.GetAllUsersAsync();
            Customers = await _customerService.GetAllAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteCustomerAsync(Guid id)
        {
            CustomerMessage = await _customerService.DeleteAsync(id);

            Users = await _authService.GetAllUsersAsync();
            Customers = await _customerService.GetAllAsync();

            return Page();
        }
    }
}