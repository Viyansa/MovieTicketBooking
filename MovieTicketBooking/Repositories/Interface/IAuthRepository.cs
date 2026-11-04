using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        Task<IdentityResult> CreateUserAsync(
            ApplicationUser user, string password);

        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);

        Task<IdentityResult> AddUserToRoleAsync(
            ApplicationUser user, string role);

        Task<bool> CheckPasswordAsync(
            ApplicationUser user, string password);

        Task<bool> RoleExistsAsync(string role);

        Task<IdentityResult> CreateRoleAsync(string role);
    }
}