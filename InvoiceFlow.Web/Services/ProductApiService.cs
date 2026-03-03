using Domain.Entities;
using InvoiceFlow.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InvoiceFlow.Web.Services
{
    public class ProductApiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _context;

        public ProductApiService(IHttpClientFactory factory, IHttpContextAccessor context)
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

        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<ProductViewModel>>("product");
        }

        public async Task<string> AddAsync(ProductViewModel model)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("product", model);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return content;

            return "Product added successfully";
        }
        public async Task<bool> AddProductAsync(Product product)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("product/add", product);

            return response.IsSuccessStatusCode;
        }
        public async Task<string> DeleteAsync(Guid id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"product/{id}");

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return content;

            return "Product deleted successfully";
        }

        public async Task<ProductViewModel> GetProductByIdAsync(Guid id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"product/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductViewModel>();
            }
            return null;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync($"product/{product.Id}", product);

            return response.IsSuccessStatusCode;
        }
    }
}