using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Authorization
{
    public class BookingOwnerAuthorizationHandler
        : AuthorizationHandler<BookingOwnerRequirement, Booking>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BookingOwnerRequirement requirement,
            Booking booking)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            string? userId = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (userId != null && userId == booking.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}