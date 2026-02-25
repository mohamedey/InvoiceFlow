using Application.DTOs;
using System.Net.Http.Headers;

namespace InvoiceFlow.Web.Services
{
    public class InvoiceApiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _httpContext;

        public InvoiceApiService(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContext)
        {
            _factory = factory;
            _httpContext = httpContext;
        }

        private HttpClient CreateClient()
        {
            var client = _factory.CreateClient("ApiClient");

            var token = _httpContext.HttpContext.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<List<InvoiceResponseDto>> GetMyAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<InvoiceResponseDto>>("invoice/my");
        }
    }
}
