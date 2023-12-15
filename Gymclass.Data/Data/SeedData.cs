using System.Security.Claims;
using GymClass.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GymClass.Data.Data
{
    public class SeedData
    {
        private static ApplicationDbContext context = default!;
        private static RoleManager<IdentityRole> roleManager = default!;
        private static UserManager<ApplicationUser> userManager = default!;

        public static async Task InitAsync(ApplicationDbContext _context, IServiceProvider serviceProvider)
        {
            context = _context;

            if (!context.GymClasses.Any())
            {
                await AddClassesAsync();

                await context.SaveChangesAsync();
            }

            if (context.Roles.Any()) return;

            roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roleNames = new[] { "User", "Admin" };
            var adminEmail = "admin@Gymbooking.com";
            //   var userEmail = "user@user.com";

            await AddRolesAsync(roleNames);

            var admin = (ApplicationUser)await AddAccountAsync(adminEmail, "Chris", "Rabb", "@Dmin123");
            // var user = await AddAccountAsync(userEmail, "Kathleen", "Johansson", 41, "K@th1986");

            await AddUserToRoleAsync(admin, "Admin");
        }

        private static async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var result = await userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task<ApplicationUser> AddAccountAsync(string accountEmail, string fName, string lName, string pw)
        {
            var findUser = await userManager.FindByEmailAsync(accountEmail);

            if (findUser != null) return null!;

            var user = new ApplicationUser
            {
                UserName = accountEmail,
                Email = accountEmail,
                FirstName = fName,
                LastName = lName,
                PasswordHash = pw,
                TimeOfRegistration = DateTime.Now,
                EmailConfirmed = true
            };

            // Add custom claims
            var claims = new List<Claim>
            {
                new Claim("FullName", $"{fName} {lName}"),
                // Add more claims as needed
            };

            var result = await userManager.CreateAsync(user, pw);

            if (result.Succeeded)
            {
                // Add claims to the user
                await userManager.AddClaimsAsync(user, claims);
            }
            else
            {
                throw new Exception(string.Join("\n", result.Errors));
            }

            return user;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task AddClassesAsync()
        {
            var gymClasses = new List<Core.Entities.GymClass>
            {
                new Core.Entities.GymClass
                {
                    Name = "Box It",
                    StartTime = new DateTime(2023,12,15, 14,30,0),
                    Duration = new TimeSpan(01,00,00),
                    Description = "Rough"
                },
                new Core.Entities.GymClass
                {
                    Name = "Badminton",
                    StartTime = new DateTime(2023,12,02, 10,00,00),
                    Duration = new TimeSpan(01,30,00),
                    Description = "High Intensity"
                },
                new Core.Entities.GymClass
                {
                    Name = "Spinning",
                    StartTime = new DateTime(2023,12,01, 18,00,00),
                    Duration = new TimeSpan(01,00,00),
                    Description = "High Intensity"
                },
                new Core.Entities.GymClass
                {
                    Name = "Body Pump",
                    StartTime = new DateTime(2023,12,15, 15,00,00),
                    Duration = new TimeSpan(00,55,00),
                    Description = "Intense Training"
                },
                new Core.Entities.GymClass
                {
                Name = "Cross Training",
                StartTime = new DateTime(2023,12,17, 08,00,00),
                Duration = new TimeSpan(00,45,00),
                Description = "High Intensity Training"
                }
            };


            await context.GymClasses.AddRangeAsync(gymClasses);
        }
    }
}
