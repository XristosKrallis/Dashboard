using Dashboard.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.Core.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var adminRole = new Role { Name = "Admin" };
            var superAdmin = new Role { Name = "SuperAdmin" };
            var userRole = new Role { Name = "User" };

            context.Roles.AddRange(adminRole, superAdmin, userRole);
            context.SaveChanges();

     
            var hasher = new PasswordHasher<User>();

            var admin = new User
            {
                Email = "admin@test.com",
                Username = "admin"
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

            var superadmin = new User
            {
                Email = "manager@test.com",
                Username = "super_admin"
            };
            superadmin.PasswordHash = hasher.HashPassword(superadmin, "Manager123!");

            var user1 = new User
            {
                Email = "user1@test.com",
                Username = "user1"
            };
            user1.PasswordHash = hasher.HashPassword(user1, "User123!");

            var user2 = new User
            {
                Email = "user2@test.com",
                Username = "user2"
            };
            user2.PasswordHash = hasher.HashPassword(user2, "User123!");

            context.Users.AddRange(admin, superadmin, user1, user2);
            context.SaveChanges();

            context.UserRoles.AddRange(
                new UserRole { UserId = admin.Id, RoleId = adminRole.Id },

                new UserRole { UserId = superadmin.Id, RoleId = superadmin.Id },

                new UserRole { UserId = user1.Id, RoleId = userRole.Id },

                new UserRole { UserId = user2.Id, RoleId = userRole.Id }
            );

            context.SaveChanges();
        }
    }
}
