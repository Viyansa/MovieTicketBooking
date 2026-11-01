using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interface
{
    public interface IScreenService
    {
        Task<ScreenResponse> CreateAsync(CreateScreenRequest request);
        Task<List<ScreenResponse>> GetAllAsync(int? theatreId);
        Task<ScreenResponse?> GetByIdAsync(int id);
        Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId);
        Task<ScreenResponse?> UpdateAsync(int id, UpdateScreenRequest request);
        Task<bool> DeleteAsync(int id);
    }
}