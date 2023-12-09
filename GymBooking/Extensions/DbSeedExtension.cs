using GymClass.Data.Data;

namespace GymBooking.WebApp.Extensions
{
    public static class DbSeedExtension
    {
        public static async Task SeedDataAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var serviceProvider = scope.ServiceProvider;
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                await SeedData.Init(db, serviceProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
