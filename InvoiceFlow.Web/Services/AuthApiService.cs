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
        public async Task<string> AdminCreateUserAsync(RegisterRequest request)
        {
            var client = _factory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("auth/register", request);

            if (!response.IsSuccessStatusCode)
                return "User creation failed";

            return "User created successfully";
        }

        public async Task<List<UserRoleViewModel>> GetAllUsersAsync()
        {
            var client = _factory.CreateClient("ApiClient");
            return await client.GetFromJsonAsync<List<UserRoleViewModel>>("auth/admin-users");
        }

        public async Task<string> UpdateRoleAsync(string userId, string role)
        {
            var client = _factory.CreateClient("ApiClient");

            var dto = new
            {
                UserId = userId,
                NewRole = role
            };

            var response = await client.PostAsJsonAsync("auth/admin-update-role", dto);

            if (!response.IsSuccessStatusCode)
                return "Role update failed";

            return "Role updated successfully";
        }
    }
}
