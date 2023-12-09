using GymBooking.Services;
using GymClass.Data.Data;
using Microsoft.EntityFrameworkCore;


namespace GymClass.Repositories.Repositories
{

    public class GymClassRepository : IGymClassRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMessageToUserService messageToUserService;

        public GymClassRepository(ApplicationDbContext context, IMessageToUserService messageToUserService)
        {
            this.context = context;
            this.messageToUserService = messageToUserService;
        }
        public async Task<List<BusinessLogic.Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false)
        {
            IQueryable<BusinessLogic.Entities.GymClass> getClasses;

            if (showHistory)
            {
                messageToUserService.AddMessage("My History");
                // Display all classes for history
                getClasses = context.GymClasses
                    .Include(m => m.AttendingMembers)
                    .ThenInclude(u => u.ApplicationUser);

                //Filter so it wont show upcoming classes in history
                getClasses = getClasses.Where(m => m.StartTime <= DateTime.Now);
            }
            else
            {
                //Only show upcoming classes
                messageToUserService.AddMessage("Overview Classes");
                // Display upcoming classes for non-history
                getClasses = context.GymClasses
                    .Where(c => c.StartTime >= DateTime.Now)
                    .Include(m => m.AttendingMembers)
                    .ThenInclude(u => u.ApplicationUser);

                // Filter so it only shows booked classes in my booked classes
                if (showBooked) messageToUserService.AddMessage("My Bookings");
                ; getClasses = showBooked
                    ? getClasses.Where(m => m.AttendingMembers.Any(u => u.ApplicationUserId == userId))
                    : getClasses;
            }

            return await getClasses.ToListAsync();
        }
    }
}
