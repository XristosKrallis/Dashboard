using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Dashboard.Web.Pages.Auth
{
    public class SettingsModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public bool ReceiveNotifications { get; set; }

        [BindProperty]
        public bool DarkMode { get; set; }

        public string StatusMessage { get; set; }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                Username = User.Identity.Name ?? User.FindFirstValue(ClaimTypes.Name) ?? "";

                Email = User.FindFirstValue(ClaimTypes.Email) ?? "";

            }
            else
            {
                RedirectToPage("/Auth/Login");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Here you would save the updated info to your database or API
            // Example:
            // _userService.UpdateSettings(User.FindFirstValue(ClaimTypes.NameIdentifier), Username, Email, NewPassword, ReceiveNotifications, DarkMode);

            StatusMessage = "Your settings have been saved successfully!";
            return Page();
        }
    }
}
