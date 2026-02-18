using Dashboard.Core.DTOs;
using Dashboard.Core.Models;

namespace Dashboard.Core.Interfaces
{
    public interface IUserService : ICrudService<User, int>
    {
        Task<UserDto> ToDtoAsync(User entity);
    }
}