using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interface
{
    public interface IScreenRepository
    {
        Task<Screen> AddAsync(Screen screen);
        Task<List<Screen>> GetAllAsync(int? theatreId);
        Task<Screen?> GetByIdAsync(int id);
        Task<List<Screen>> GetByTheatreIdAsync(int theatreId);
        Task<bool> ExistsByNameAsync(int theatreId, string screenName, int? excludeScreenId = null);
        Task UpdateAsync(Screen screen);
        Task<bool> DeleteAsync(int id);
    }
}