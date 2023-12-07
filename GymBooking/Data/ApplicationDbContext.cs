using GymBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GymClass> GymClasses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            //make composite key
            builder.Entity<ApplicationUserGymClass>()
                .HasKey(t => new { t.ApplicationUserId, t.GymClassId });
            //make shadow property
            builder.Entity<ApplicationUser>()
                .Property<DateTime>("TimeOfRegistration");

            base.OnModelCreating(builder);
        }
    }
}
