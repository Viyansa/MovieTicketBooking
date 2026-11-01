using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Controllers
{
    [Route("api/screens")]
    [ApiController]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateScreenRequest request)
        {
            try
            {
                var screen = await _screenService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = screen.ScreenId },
                    screen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Unable to create screen due to a database constraint." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? theatreId)
        {
            var screens = await _screenService.GetAllAsync(theatreId);

            return Ok(screens);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var screen = await _screenService.GetByIdAsync(id);

            if (screen == null)
            {
                return NotFound(new { message = "Screen not found." });
            }

            return Ok(screen);
        }

        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetByTheatreId(int theatreId)
        {
            try
            {
                var screens = await _screenService.GetByTheatreIdAsync(theatreId);

                return Ok(screens);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateScreenRequest request)
        {
            try
            {
                var screen = await _screenService.UpdateAsync(id, request);

                if (screen == null)
                {
                    return NotFound(new { message = "Screen not found." });
                }

                return Ok(screen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Unable to update screen due to a database constraint." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _screenService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Screen not found." });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Cannot delete this screen because dependent records exist."
                });
            }
        }
    }
}
