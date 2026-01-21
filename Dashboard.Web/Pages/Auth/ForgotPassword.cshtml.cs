using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Web.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool accountExists = FakeUserCheck(Email);

            return RedirectToPage("/Auth/ForgotPasswordResult", new { isSuccess = accountExists });
        }

        private bool FakeUserCheck(string email)
        {
            var existingEmails = new List<string> { "user1@test.com", "user2@test.com" };
            return existingEmails.Contains(email.ToLower());
        }
    }
}
