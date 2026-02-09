
using Dashboard.Core.Data;
using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = u.UserRoles
                        .Select(r => r.Role.Name)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<UserDto> CreateAsync(UserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Default123!")
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var rolesToAssign = (dto.Roles == null || dto.Roles.Count == 0)
                        ? new List<string> { "User" }
                        : dto.Roles;

            await AssignRolesAsync(user.Id, rolesToAssign);

            return await GetUserDtoAsync(user.Id);
        }

        public async Task<UserDto?> UpdateAsync(int id, UserDto dto)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            user.Username = dto.Username;
            user.Email = dto.Email;

            _db.UserRoles.RemoveRange(user.UserRoles);

            await AssignRolesAsync(user.Id, dto.Roles);
            await _db.SaveChangesAsync();

            return await GetUserDtoAsync(user.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return false;

            _db.UserRoles.RemoveRange(user.UserRoles);
            _db.Users.Remove(user);

            await _db.SaveChangesAsync();
            return true;
        }

        private async Task AssignRolesAsync(int userId, List<string>? roleNames)
        {
            if (roleNames == null || roleNames.Count == 0)
                return;

            var roles = await _db.Roles
                .Where(r => roleNames.Contains(r.Name))
                .ToListAsync();

            _db.UserRoles.AddRange(
                roles.Select(r => new UserRole
                {
                    UserId = userId,
                    RoleId = r.Id
                })
            );

            await _db.SaveChangesAsync();
        }

        private async Task<UserDto> GetUserDtoAsync(int userId)
        {
            return await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = u.UserRoles
                        .Select(r => r.Role.Name)
                        .ToList()
                })
                .FirstAsync();
        }
    }
}
