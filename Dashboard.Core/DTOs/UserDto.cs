
namespace Dashboard.Core.DTOs
{
    public class UserDto
    {
        public int? Id { get; set; } 
        public string? Username { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }

    public sealed class UserIdentity
    {
        public string Id { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Username { get; init; } = default!;
        public IReadOnlyCollection<string> Roles { get; init; } = [];
    }

}
