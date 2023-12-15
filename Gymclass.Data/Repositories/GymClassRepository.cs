using System.Security.Claims;
using GymClass.Core.Entities;
using GymClass.Core.Repositories;
using GymClass.Data.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace GymClass.Data.Repositories
{
    public class GymClassRepository : IGymClassRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GymClassRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Core.Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false)
        {
            IQueryable<Core.Entities.GymClass> getClasses;

            if (showHistory)
            {
                // Display all classes for history
                getClasses = context.GymClasses
                    .Where(m => m.StartTime <= DateTime.Now);
            }
            else
            {
                //Only show upcoming classes
                getClasses = context.GymClasses
                    .Where(c => c.StartTime >= DateTime.Now)
                    .Include(m => m.AttendingMembers)
                    .ThenInclude(u => u.ApplicationUser);

                // Filter so it only shows booked classes in my booked classes
                getClasses = showBooked
                  ? getClasses.Where(m => m.AttendingMembers.Any(u => u.ApplicationUserId == userId))
                  : getClasses;
            }

            return await getClasses.ToListAsync();
        }

        public async Task<Core.Entities.GymClass> GetAsync(int id)
        {

            var gymClass = await context.GymClasses
                .Include(m => m.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);


            return gymClass!;
        }
        public bool Any(int? id)
        {
            return context.GymClasses.Any(i => i.Id == id);
        }

        public void Remove(Core.Entities.GymClass gymClass)
        {
            context.Remove(gymClass);
        }

        public void Update(Core.Entities.GymClass gymClass)
        {
            context.Update(gymClass);
        }

        public void Add(Core.Entities.GymClass gymClass)
        {
            context.Add(gymClass);
        }

        public async Task<Core.Entities.GymClass> BookingToggleAsync(int? id)
        {

            var gymClass = await context.GymClasses
                .Include(m => m.AttendingMembers)
                .FirstOrDefaultAsync(g => g.Id == id);


            // this code is retrieving the unique identifier of the currently authenticated user from the claims in the HTTP context
            var currentUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


            //Is the user already attending
            var attendingMember = gymClass!.AttendingMembers
                .FirstOrDefault(member => member.ApplicationUserId == currentUser);

            if (attendingMember == null)
            {
                gymClass.AttendingMembers.Add(new ApplicationUserGymClass
                {
                    ApplicationUserId = currentUser,
                    GymClassId = gymClass.Id

                });
            }
            else
            {
                gymClass.AttendingMembers.Remove(attendingMember);
            }

            return gymClass;
        }

        public async Task<IList<Core.Entities.GymClass>> MyBookingHistoryAsync()
        {

            var currentUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myBookingHistory = context.Users
                .Where(u => u.Id == currentUser)
                .SelectMany(g => g.AttendingClasses)
                .Select(gy => gy.GymClass)
                .Where(d => d.StartTime < DateTime.Now);

            return await myBookingHistory.ToListAsync();
        }
    }
}
