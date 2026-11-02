
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<MovieResponse> CreateAsync(CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        await _movieRepository.AddAsync(movie);

        return MapToResponse(movie);
    }

    public async Task<List<MovieResponse>> GetAllAsync(
        string? title, int pageNumber, int pageSize)
    {
        var movies = await _movieRepository.GetAllAsync(
            title, pageNumber, pageSize);

        return movies.Select(MapToResponse).ToList();
    }

    public async Task<MovieResponse?> GetByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
            return null;

        return MapToResponse(movie);
    }

    public async Task<List<MovieResponse>> SearchAsync(string title)
    {
        var movies = await _movieRepository.SearchAsync(title);

        return movies.Select(MapToResponse).ToList();
    }

    public async Task<MovieResponse?> UpdateAsync(
        int id, UpdateMovieRequest request)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
            return null;

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language;
        movie.Genre = request.Genre;

        await _movieRepository.UpdateAsync(movie);

        return MapToResponse(movie);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
            return false;

        await _movieRepository.DeleteAsync(movie);

        return true;
    }

    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}

