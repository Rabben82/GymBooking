﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymClass.BusinessLogic.Entities;
using GymClass.Data.Data;
using Microsoft.EntityFrameworkCore;
using GymClass.BusinessLogic.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using GymBooking.WebApp.Services;

namespace GymClass.Tests.Repositories.Fixtures
{
    public class TestDatabaseFixture : IDisposable
    {
        public ApplicationDbContext Context { get; set; } = null!;
        public IMessageToUserService MessageToUserService { get; private set; }
        public IHttpContextAccessor HttpContextAccessor { get; private set; }

        public TestDatabaseFixture()
        {
            MessageToUserService = new MessageToUserService();
            HttpContextAccessor = SetupHttpContextAccessor();
            SeedData();
        }

        private IHttpContextAccessor SetupHttpContextAccessor()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var user = new ApplicationUser
            {
                Id = "expectedUserId",
                FirstName = "Chris",
                LastName = "Rabb",
            };

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                // Add more claims as needed
            });

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(claimsPrincipal);

            return httpContextAccessorMock.Object;
        }

        private void SeedData()
        {
            // Create a test database
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

            Context.GymClasses.Add(new BusinessLogic.Entities.GymClass
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
