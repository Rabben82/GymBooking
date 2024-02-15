using GymClass.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using GymClass.Core.Entities;

namespace GymClass.Tests.Repositories.Fixtures
{
    public class TestDatabaseFixture : IDisposable
    {
        public ApplicationDbContext Context { get; set; } = null!;
        public IHttpContextAccessor HttpContextAccessor { get; private set; }

        public TestDatabaseFixture()
        {
            HttpContextAccessor = SetupHttpContextAccessor();
            SeedData();
        }

        private IHttpContextAccessor SetupHttpContextAccessor()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            //User Setup
            var user = new ApplicationUser
            {
                Id = "expectedUserId",
                FirstName = "Chris",
                LastName = "Rabb",
            };

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            });

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //The mock is configured to return this predefined user when the HttpContext.User property is accessed.
            httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(claimsPrincipal);

            return httpContextAccessorMock.Object;
        }

        private void SeedData()
        {
            var testDb = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            Context = new ApplicationDbContext(testDb);

            Context.Database.Migrate();

            Context.Users.Add(new ApplicationUser
            {
                Id = "expectedUserId",
                FirstName = "Chris",
                LastName = "Rabb",
            });

            Context.GymClasses.Add(new Core.Entities.GymClass
            {
                Name = "Tennis",
                StartTime = new DateTime(2023, 12, 17, 08, 00, 00),
                Duration = new TimeSpan(01, 00, 00),
            });

            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
