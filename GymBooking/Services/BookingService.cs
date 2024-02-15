using System.Security.Claims;
using GymClass.Core.Services;
using GymClass.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.WebApp.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext context;

        public BookingService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> IsMemberBooked(ClaimsPrincipal user, int gymClassId)
        {
            var currentUser = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUser != null)
            {
                var isBooked = await context.GymClasses
                    .AnyAsync(g => g.Id == gymClassId && g.AttendingMembers
                        .Any(m => m.ApplicationUserId == currentUser));
                //var isBooked = await context.ApplicationUsers
                //    .Where(au => au.AttendingClasses.Contains(gymClassId))
                //    .AnyAsync();

                return isBooked;
            }

            return false;
        }
    }
}
