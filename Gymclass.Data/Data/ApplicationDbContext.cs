using GymClass.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymClass.Data.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Core.Entities.GymClass> GymClasses { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Specify the composite primary key for the join table.
            builder.Entity<ApplicationUserGymClass>()
                .HasKey(t => new { t.ApplicationUserId, t.GymClassId });

            //Establishing the many-to-many relationship between ApplicationUser and GymClass
            builder.Entity<ApplicationUserGymClass>()
                .HasOne(au => au.ApplicationUser)
                .WithMany(g => g.AttendingClasses)
                .HasForeignKey(u => u.ApplicationUserId);

            builder.Entity<ApplicationUserGymClass>()
                .HasOne(g => g.GymClass)
                .WithMany(au => au.AttendingMembers)
                .HasForeignKey(u => u.GymClassId);

            //make shadow property
            builder.Entity<ApplicationUser>()
                .Property<DateTime>("TimeOfRegistration");

            base.OnModelCreating(builder);
        }
    }
}
