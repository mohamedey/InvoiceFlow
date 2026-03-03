using System.Net.Http.Headers;
using InvoiceFlow.Web.Models;

namespace InvoiceFlow.Web.Services
{
    public class CustomerApiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _context;

        public CustomerApiService(IHttpClientFactory factory,
                                  IHttpContextAccessor context)
        {
            _factory = factory;
            _context = context;
        }

        private HttpClient CreateClient()
        {
            var client = _factory.CreateClient("ApiClient");
            var token = _context.HttpContext.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<List<CustomerViewModel>> GetAllAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<CustomerViewModel>>("customer");
        }

        public async Task<string> CreateAsync(CustomerViewModel model)
        {
            var client = CreateClient();

            var response = await client.PostAsJsonAsync("customer", model);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return content;

            return "Customer created successfully";
        }

        public async Task<string> DeleteAsync(Guid id)
        {
            var client = CreateClient();

            var response = await client.DeleteAsync($"customer/{id}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return content;

            return "Customer deleted successfully";
        }
    }
}