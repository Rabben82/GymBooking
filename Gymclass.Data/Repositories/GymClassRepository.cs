using GymClass.BusinessLogic.Exceptions;
using GymClass.BusinessLogic.Repositories;
using GymClass.BusinessLogic.Services;
using GymClass.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace GymClass.Data.Repositories
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

        public async Task<BusinessLogic.Entities.GymClass> GetAsync(int id, string message)
        {
            messageToUserService.AddMessage(message);

            var gymClass = await context.GymClasses
                .Include(m => m.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gymClass == null)
            {
                // You might log a message, throw an exception, or handle it in a way that suits your application logic.
                // For example, throw an exception:
                throw new EntityNotFoundException($"GymClass with id {id} not found");
            }

            return gymClass;
        }

        public void Remove(BusinessLogic.Entities.GymClass gymClass)
        {
            context.Remove(gymClass);
        }

        public void Update(BusinessLogic.Entities.GymClass gymClass)
        {
            context.Update(gymClass);
        }
    }
}
