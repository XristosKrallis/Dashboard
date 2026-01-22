using Dashboard.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Dashboard.Core.Services;
using Dashboard.Core.Interfaces;

namespace Dashboard.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;

        public RegisterModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty][Required] public string Username { get; set; }

        [BindProperty][Required][EmailAddress] public string Email { get; set; }

        [BindProperty][Required][DataType(DataType.Password)] public string Password { get; set; }

        [BindProperty][Required] [Compare("Password", ErrorMessage = "Passwords do not match.")] [DataType(DataType.Password)] public string ConfirmPassword { get; set; }

        [BindProperty] public string Country { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        //An thelw na pros8esw sto register, dto, form klp klp
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authService.RegisterAsync(new RegisterRequest
            {
                Email = Email,
                Username = Username,
                Password = Password
            });

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            //An ginei kati stin basi na epistrefw false
            return RedirectToPage("/Auth/RegisterResult", new { IsSuccess = true });
        }
    }
}
