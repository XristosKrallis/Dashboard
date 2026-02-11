using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Dashboard.Core.DTOs;

namespace Dashboard.Web.Pages.Auth
{
    public class SettingsModel : PageModel
    {
        private readonly IAuthService _authService;

        public SettingsModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [BindProperty]
        public bool ReceiveNotifications { get; set; }

        [BindProperty]
        public bool DarkMode { get; set; }

        public string? StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToPage("/Auth/Login");

            Username = User.Identity?.Name
                       ?? User.FindFirstValue(ClaimTypes.Name)
                       ?? string.Empty;

            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToPage("/Auth/Login");

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError(nameof(ConfirmPassword),
                        "Passwords do not match.");
                }
            }

            if (!ModelState.IsValid)
                return Page();

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return RedirectToPage("/Auth/Login");
            }

            var result = await _authService.UpdateAsync(new UpdateUserRequest
            {
                UserId = userId,
                Username = Username,
                Email = Email,
                NewPassword = NewPassword
            });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Update failed.");
                return Page();
            }

            NewPassword = null;
            ConfirmPassword = null;

            StatusMessage = "Your settings have been saved successfully!";
            return Page();
        }
    }
}
