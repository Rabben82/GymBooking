using GymClass.BusinessLogic.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GymClass.Data.Data
{
    public class SeedData
    {
        private static ApplicationDbContext context = default!;
        private static RoleManager<IdentityRole> roleManager = default!;
        private static UserManager<ApplicationUser> userManager = default!;

        public static async Task Init(ApplicationDbContext _context, IServiceProvider serviceProvider)
        {
            context = _context;

            if (context.Roles.Any()) return;

            roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roleNames = new[] { "User", "Admin" };
            var adminEmail = "admin@Gymbooking.com";
         //   var userEmail = "user@user.com";

            await AddRolesAsync(roleNames);

            var admin = (ApplicationUser)await AddAccountAsync(adminEmail, "Chris","Rabb","R@bben1982");
           // var user = await AddAccountAsync(userEmail, "Kathleen", "Johansson", 41, "K@th1986");

           await AddUserToRoleAsync(admin, "Admin");
        }

        private static async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var result = await userManager.AddToRoleAsync(user, roleName);
                if(!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
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
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, pw);

            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

            return user;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if(await roleManager.RoleExistsAsync(roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }
    }
}
