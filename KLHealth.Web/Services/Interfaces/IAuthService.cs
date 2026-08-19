using KLHealth.Web.Models.ViewModels;

namespace KLHealth.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Role, string Message)> LoginAsync(LoginViewModel model);
        Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model);
        Task LogoutAsync();
    }
}