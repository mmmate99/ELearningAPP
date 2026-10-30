// Services/Implementations/AuthService.cs - TELJES JAVÍTOTT verzió
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger; // Logging hozzáadása

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

                // Ellenőrizd, hogy az email már létezik-e
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
                    throw new Exception("Email already registered");
                }

                // Új felhasználó létrehozása
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Role = registerDto.Role,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Creating user: {Email}", registerDto.Email);
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("User creation failed: {Errors}", errors);
                    throw new Exception($"Registration failed: {errors}");
                }

                _logger.LogInformation("User created successfully: {Email}", registerDto.Email);

                // Szerepkör hozzáadása
                var roleName = registerDto.Role.ToString();
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Failed to add role {Role} to user {Email}", roleName, registerDto.Email);
                }

                // JWT token generálása
                var token = GenerateJwtToken(user);
                
                _logger.LogInformation("Registration successful for: {Email}", registerDto.Email);

                return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    User = _mapper.Map<UserDto>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", registerDto.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                
                if (user == null)
                {
                    _logger.LogWarning("Login failed - user not found: {Email}", loginDto.Email);
                    throw new Exception("Invalid email or password");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                
                if (!passwordValid)
                {
                    _logger.LogWarning("Login failed - invalid password for: {Email}", loginDto.Email);
                    throw new Exception("Invalid email or password");
                }

                // JWT token generálása
                var token = GenerateJwtToken(user);
                
                _logger.LogInformation("Login successful for: {Email}", loginDto.Email);

                return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    User = _mapper.Map<UserDto>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                return user != null ? _mapper.Map<UserDto>(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
                
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.GivenName, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user: {UserId}", user.Id);
                throw;
            }
        }
    }
}