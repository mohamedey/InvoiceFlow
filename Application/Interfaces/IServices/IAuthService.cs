using Application.DTOs.Auth;


namespace Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<(bool Success, string Message)> UpdateRoleAsync(string userId, string newRole, string currentAdminId);
        Task<List<UserRoleDto>> GetAllUsersAsync();
    }
}
