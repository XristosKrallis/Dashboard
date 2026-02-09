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
        private readonly IJwtService _jwtService;

        public LoginModel(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
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

            var identity = result.Identity!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identity.Id),
                new Claim(ClaimTypes.Email, identity.Email),
                new Claim(ClaimTypes.Name, identity.Username)
            };

            foreach (var role in identity.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            var jwt = _jwtService.CreateToken(identity);

            return RedirectToPage("/Index");
        }
    }
}
