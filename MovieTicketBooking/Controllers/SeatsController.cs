using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Controllers
{
    [Route("api/seats")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSeatRequest request)
        {
            try
            {
                var seat = await _seatService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = seat.SeatId },
                    seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "This seat already exists or violates a database constraint."
                });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(BulkCreateSeatsRequest request)
        {
            try
            {
                var seats = await _seatService.BulkCreateAsync(request);

                return StatusCode(StatusCodes.Status201Created, seats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to create seats due to a database constraint."
                });
            }
        }

        [HttpGet("screen/{screenId}")]
        public async Task<IActionResult> GetByScreenId(int screenId)
        {
            try
            {
                var seats = await _seatService.GetByScreenIdAsync(screenId);

                return Ok(seats);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
            {
                return NotFound(new { message = "Seat not found." });
            }

            return Ok(seat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSeatRequest request)
        {
            try
            {
                var seat = await _seatService.UpdateAsync(id, request);

                if (seat == null)
                {
                    return NotFound(new { message = "Seat not found." });
                }

                return Ok(seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to update seat due to a database constraint."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _seatService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Seat not found." });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Cannot delete this seat because dependent show or booking records exist."
                });
            }
        }
    }
}