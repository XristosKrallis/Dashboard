using Dashboard.Core.Data;
using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _hasher;

        public AuthService(AppDbContext db, IPasswordHasher<User> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password."
                };

            var verify = _hasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password);

            if (verify != PasswordVerificationResult.Success)
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password."
                };

            return new LoginResult
            {
                Success = true,
                Identity = new UserIdentity
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    Roles = user.UserRoles
                        .Select(r => r.Role.Name)
                        .ToList()
                }
            };
        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Email already exists."
                };

            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
            };

            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = _db.Roles.First(r => r.Name == "User").Id
            };

            _db.UserRoles.Add(userRole);
            await _db.SaveChangesAsync();

            return new RegisterResult
            {
                Success = true,
                UserId = user.Id
            };
        }

        public async Task<UpdateResult> UpdateAsync(UpdateUserRequest request)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return new UpdateResult
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }

            var emailExists = await _db.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId);

            if (emailExists)
            {
                return new UpdateResult
                {
                    Success = false,
                    ErrorMessage = "Email already exists."
                };
            }

            user.Username = request.Username;
            user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                user.PasswordHash = _hasher.HashPassword(user, request.NewPassword);
            }

            await _db.SaveChangesAsync();

            return new UpdateResult
            {
                Success = true
            };
        }
    }
}
