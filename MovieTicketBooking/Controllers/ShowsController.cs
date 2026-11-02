using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Controllers
{
    [Route("api/shows")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShowRequest request)
        {
            try
            {
                var show = await _showService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = show.ShowId },
                    show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to create show due to a database constraint."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shows = await _showService.GetAllAsync();
            return Ok(shows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var show = await _showService.GetByIdAsync(id);

            if (show == null)
            {
                return NotFound(new { message = "Show not found." });
            }

            return Ok(show);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetByMovieId(int movieId)
        {
            var shows = await _showService.GetByMovieIdAsync(movieId);
            return Ok(shows);
        }

        [HttpGet("screen/{screenId}")]
        public async Task<IActionResult> GetByScreenId(int screenId)
        {
            var shows = await _showService.GetByScreenIdAsync(screenId);
            return Ok(shows);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateShowRequest request)
        {
            try
            {
                var show = await _showService.UpdateAsync(id, request);

                if (show == null)
                {
                    return NotFound(new { message = "Show not found." });
                }

                return Ok(show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to update show due to a database constraint."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _showService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Show not found." });
            }

            return NoContent();
        }
    }
}