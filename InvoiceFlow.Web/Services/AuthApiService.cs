using InvoiceFlow.Web.Models;

namespace InvoiceFlow.Web.Services
{
    public class AuthApiService
    {
        private readonly IHttpClientFactory _factory;

        public AuthApiService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var client = _factory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("auth/login", request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login failed");

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
    }
}
