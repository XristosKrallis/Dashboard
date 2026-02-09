using Dashboard.Core.DTOs;

namespace Dashboard.Core.Interfaces
{
    public interface IJwtService
    {
        string CreateToken(UserIdentity identity);
    }
}
