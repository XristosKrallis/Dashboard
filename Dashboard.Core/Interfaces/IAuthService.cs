using Dashboard.Core.DTOs;

namespace Dashboard.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
        Task<UpdateResult> UpdateAsync(UpdateUserRequest request);
        //Task<bool> ForgotPasswordAsync(string email);
    }
}
