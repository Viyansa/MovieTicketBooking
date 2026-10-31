
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Route("api/movies")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovie(CreateMovieRequest request)
    {
        var movie = await _movieService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetMovieById),
            new { id = movie.MovieId },
            movie);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMovies(
        [FromQuery] string? title,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination values.");

        var movies = await _movieService.GetAllAsync(
            title, pageNumber, pageSize);

        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMovieById(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);

        if (movie == null)
            return NotFound("Movie not found.");

        return Ok(movie);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(
        [FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Please enter a movie title.");

        var movies = await _movieService.SearchAsync(title);

        return Ok(movies);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(
        int id, UpdateMovieRequest request)
    {
        var movie = await _movieService.UpdateAsync(id, request);

        if (movie == null)
            return NotFound("Movie not found.");

        return Ok(movie);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var result = await _movieService.DeleteAsync(id);

        if (!result)
            return NotFound("Movie not found.");

        return NoContent();
    }
}

