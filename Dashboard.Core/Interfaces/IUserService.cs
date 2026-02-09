using Dashboard.Core.DTOs;

namespace Dashboard.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> CreateAsync(UserDto dto);
        Task<UserDto?> UpdateAsync(int id, UserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}