using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interface
{
    public interface IMovieRepository
    {
        Task<Movie> AddAsync(Movie movie);

        Task<List<Movie>> GetAllAsync();

        Task<List<Movie>> GetAllAsync(
            string? title,
            int pageNumber,
            int pageSize);

        Task<Movie?> GetByIdAsync(int id);

        Task<List<Movie>> SearchAsync(string title);

        Task UpdateAsync(Movie movie);

        Task DeleteAsync(Movie movie);

        Task<bool> DeleteAsync(int id);
    }
}