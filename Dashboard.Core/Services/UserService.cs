using Dashboard.Core.Data;
using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService : CrudService<User, int>, IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();

        return users;
    }

    public override async Task<User> CreateAsync(User entity)
    {
        if (await _db.Users.AnyAsync(u => u.Username == entity.Username))
            throw new InvalidOperationException("Username already exists.");

        if (await _db.Users.AnyAsync(u => u.Email == entity.Email))
            throw new InvalidOperationException("Email already exists.");

        entity.PasswordHash = new PasswordHasher<User>().HashPassword(entity, "Default123!");

        await base.CreateAsync(entity);

        var rolesToAssign = entity.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string> { "User" };
        await AssignRolesAsync(entity.Id, rolesToAssign);

        return entity;
    }

    public override async Task<User> UpdateAsync(User entity)
    {
        var user = await _db.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == entity.Id);
        if (user == null) throw new InvalidOperationException("User not found.");

        if (!string.IsNullOrWhiteSpace(entity.Username) &&
            await _db.Users.AnyAsync(u => u.Username == entity.Username && u.Id != entity.Id))
            throw new InvalidOperationException("Username already exists.");

        if (!string.IsNullOrWhiteSpace(entity.Email) &&
            await _db.Users.AnyAsync(u => u.Email == entity.Email && u.Id != entity.Id))
            throw new InvalidOperationException("Email already exists.");

        user.Username = entity.Username ?? user.Username;
        user.Email = entity.Email ?? user.Email;

        _db.UserRoles.RemoveRange(user.UserRoles);
        await AssignRolesAsync(user.Id, entity.UserRoles?.Select(ur => ur.Role.Name).ToList());

        await base.UpdateAsync(user);
        return user;
    }

    public async Task<UserDto> ToDtoAsync(User entity)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstAsync(u => u.Id == entity.Id);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(r => r.Role.Name).ToList()
        };
    }

    private async Task AssignRolesAsync(int userId, List<string>? roleNames)
    {
        if (roleNames == null || !roleNames.Any())
            return;

        var existingRoles = _db.UserRoles.Where(ur => ur.UserId == userId);
        _db.UserRoles.RemoveRange(existingRoles);

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
}
