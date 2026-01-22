using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Dashboard.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty, Required]
        public string Email { get; set; }

        [BindProperty, Required]
        public string Password { get; set; }

        [BindProperty]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authService.LoginAsync(new LoginRequest
            {
                Email = Email,
                Password = Password,
                RememberMe = RememberMe
            });

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.User.Email),
                new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString())
            };

            foreach (var role in result.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                });

            return RedirectToPage("/Index");
        }
    }
}
