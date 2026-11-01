using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/theatres")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _service;

        public TheatresController(ITheatreService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTheatre(CreateTheatreRequest request)
        {
            var result = await _service.CreateAsync(request);

            return CreatedAtAction(nameof(GetTheatreById),
                new { id = result.TheatreId }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTheatres(
            int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid pagination values.");
            }

            var result = await _service.GetAllAsync(pageNumber, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTheatreById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound("Theatre not found.");
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheatre(
            int id, UpdateTheatreRequest request)
        {
            var result = await _service.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound("Theatre not found.");
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheatre(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                return NotFound("Theatre not found.");
            }

            return NoContent();
        }
    }
}
