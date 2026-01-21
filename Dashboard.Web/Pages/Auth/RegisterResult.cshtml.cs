using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard.Web.Pages.Auth
{
    public class RegisterResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool IsSuccess { get; set; }

        public void OnGet()
        {
            
        }
    }
}
