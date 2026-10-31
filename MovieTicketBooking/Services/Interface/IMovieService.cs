
using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> CreateAsync(CreateMovieRequest request);
    Task<List<MovieResponse>> GetAllAsync(
        string? title, int pageNumber, int pageSize);
    Task<MovieResponse?> GetByIdAsync(int id);
    Task<List<MovieResponse>> SearchAsync(string title);
    Task<MovieResponse?> UpdateAsync(int id, UpdateMovieRequest request);
    Task<bool> DeleteAsync(int id);
}

