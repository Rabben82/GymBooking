using GymClass.BusinessLogic.Entities;
using GymClass.BusinessLogic.Exceptions;
using GymClass.BusinessLogic.Repositories;
using GymClass.BusinessLogic.Services;
using GymClass.Data.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace GymClass.Data.Repositories
{
    public class GymClassRepository : IGymClassRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMessageToUserService messageToUserService;

        public GymClassRepository(ApplicationDbContext context, IMessageToUserService messageToUserService, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.messageToUserService = messageToUserService;
        }
        public async Task<List<BusinessLogic.Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false)
        {
            IQueryable<BusinessLogic.Entities.GymClass> getClasses;

            if (showHistory)
            {
                messageToUserService.AddMessage("Classes History");
                // Display all classes for history
                getClasses = context.GymClasses
                    .Where(m => m.StartTime <= DateTime.Now);
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
                  getClasses = showBooked
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


            return gymClass!;
        }
        public bool Any(int? id)
        {
            if (id == null) throw new EntityNotFoundException("GymClass with id {id} not found");
            return context.GymClasses.Any(i => i.Id == id);
        }

        public void Remove(BusinessLogic.Entities.GymClass gymClass)
        {
            context.Remove(gymClass);
        }

        public void Update(BusinessLogic.Entities.GymClass gymClass)
        {
            context.Update(gymClass);
        }

        public void Add(BusinessLogic.Entities.GymClass gymClass)
        {
            context.Add(gymClass);
        }

        public void AddMessageToUser(string message)
        {
            messageToUserService.AddMessage(message);
        }

        public async Task<BusinessLogic.Entities.GymClass> BookingToggle(int? id, ApplicationUser user)
        {
            if (id == null) throw new EntityNotFoundException($"GymClass with id {id} not found");

            var gymClass = await context.GymClasses
                .Include(m => m.AttendingMembers)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gymClass == null) throw new EntityNotFoundException($"GymClass entity not found");

            var currentUser = user;

            if (currentUser == null) throw new EntityNotFoundException($"Current user entity not found");

            //Is the user already attending
            var attendingMember = gymClass.AttendingMembers
                .FirstOrDefault(member => member.ApplicationUserId == currentUser.Id);

            if (attendingMember == null)
            {
                gymClass.AttendingMembers.Add(new ApplicationUserGymClass
                {
                    ApplicationUserId = currentUser.Id,
                    GymClassId = gymClass.Id

                });
            }
            else
            {
                gymClass.AttendingMembers.Remove(attendingMember);
            }

            return gymClass;
        }

        public async Task<IList<BusinessLogic.Entities.GymClass>> MyBookingHistory(string user, string message)
        {
            messageToUserService.AddMessage(message);

            var currentUser = user;

            var myBookingHistory = context.Users
                .Where(u => u.Id == currentUser)
                .SelectMany(g => g.AttendingClasses)
                .Select(gy => gy.GymClass)
                .Where(d => d.StartTime < DateTime.Now);

            return await myBookingHistory.ToListAsync();

        }
 
       
    }
}
