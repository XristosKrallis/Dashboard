using Dashboard.Core.Models;

namespace Dashboard.Core.DTOs
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public User User { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
