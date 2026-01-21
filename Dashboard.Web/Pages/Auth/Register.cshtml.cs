using Dashboard.Core.Data;
using Dashboard.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RegisterModel(AppDbContext db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Country { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            ErrorMessage = null;

            if (!ModelState.IsValid)
                return Page();

            if (_db.Users.Any(u => u.Email.ToLower() == Email.ToLower()))
            {
                ErrorMessage = "Email is already registered.";
                return Page();
            }

            var user = new User
            {
                Username = Username,
                Email = Email,
                Country = Country 
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, Password);

            var role = _db.Roles.FirstOrDefault(r => r.Name == "User");
            if (role != null)
            {
                _db.UserRoles.Add(new UserRole
                {
                    User = user,
                    Role = role
                });
            }

            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToPage("/Auth/RegisterResult", new { isSuccess = true });
        }
    }
}
