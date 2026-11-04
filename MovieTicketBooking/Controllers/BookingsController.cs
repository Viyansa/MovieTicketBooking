using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IAuthorizationService _authorizationService;

        public BookingsController(
            IBookingService bookingService,
            IAuthorizationService authorizationService)
        {
            _bookingService = bookingService;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateBookingRequest request)
        {
            try
            {
                string? userId = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User ID not found in token."
                    });
                }

                BookingResponse response =
                    await _bookingService.CreateAsync(request, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.BookingId },
                    response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            List<BookingResponse> bookings =
                await _bookingService.GetAllAsync();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking =
                await _bookingService.GetBookingEntityByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            var result = await _authorizationService.AuthorizeAsync(
                User,
                booking,
                "BookingOwnerOrAdmin");

            if (!result.Succeeded)
            {
                return Forbid();
            }

            BookingResponse? response =
                await _bookingService.GetByIdAsync(id);

            return Ok(response);
        }

        [HttpGet("show/{showId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByShowId(int showId)
        {
            List<BookingResponse> bookings =
                await _bookingService.GetByShowIdAsync(showId);

            return Ok(bookings);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking =
                await _bookingService.GetBookingEntityByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            var result = await _authorizationService.AuthorizeAsync(
                User,
                booking,
                "BookingOwnerOrAdmin");

            if (!result.Succeeded)
            {
                return Forbid();
            }

            try
            {
                BookingResponse? response =
                    await _bookingService.CancelAsync(id);

                if (response == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}