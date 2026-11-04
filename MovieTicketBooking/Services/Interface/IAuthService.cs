using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}