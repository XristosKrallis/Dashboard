using Dashboard.Core.Models;

namespace Dashboard.Core.DTOs
{
    public class LoginResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public UserIdentity? Identity { get; init; }
    }
}
