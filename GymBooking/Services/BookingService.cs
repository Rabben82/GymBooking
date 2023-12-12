using System.Security.Claims;
using GymClass.BusinessLogic.Services;
using GymClass.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.WebApp.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;


        public BookingService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsMemberBooked(int gymClassId)
        {
            var currentUser =  httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUser != null)
            {
                var isBooked = await context.GymClasses
                    .AnyAsync(g => g.Id == gymClassId && g.AttendingMembers
                        .Any(m => m.ApplicationUserId == currentUser));
                return isBooked;
            }

            return false;
        }
    }
}
