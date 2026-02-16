namespace Dashboard.Core.DTOs
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? NewPassword { get; set; }
    }
}
