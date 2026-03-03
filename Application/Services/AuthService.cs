using Application.DTOs.Auth;
using Application.Interfaces.IServices;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = token,
                Role = "User"
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("Invalid credentials");

            if (string.IsNullOrEmpty(dto.Password))
                throw new Exception("Password required");

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!validPassword)
                throw new Exception("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = token,
                Role = role
            };
        }
        public async Task<List<UserRoleDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserRoleDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserRoleDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return result;
        }

        public async Task<(bool Success, string Message)> UpdateRoleAsync(
    string userId,
    string newRole,
    string currentAdminId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return (false, "User not found");

            if (userId == currentAdminId && newRole != "Admin")
                return (false, "You cannot remove your own Admin role");

            if (!await _roleManager.RoleExistsAsync(newRole))
                return (false, "Role does not exist");

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            return (true, "Role updated successfully");
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = jwtSettings["Key"];

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
