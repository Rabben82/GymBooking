using GymClass.BusinessLogic.Entities;
using GymClass.BusinessLogic.Services;
using GymClass.Data.Data;
using GymClass.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using GymClass.Tests.Repositories.Fixtures;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymClass.Tests.Repositories
{
    public class GymClassRepositoryTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture fixture;
        private readonly GymClassRepository sut;

        public GymClassRepositoryTests(TestDatabaseFixture fixture)
        {
            this.fixture = fixture;

            sut = new GymClassRepository(fixture.Context, fixture.MessageToUserService, fixture.HttpContextAccessor);
        }

        [Theory]
        [InlineData(1)]
        public async Task BookingToggle_ShouldToggleBooking(int gymClassId)
        {
            //Arrange

            // Act
            var result = await sut.BookingToggle(gymClassId);

            // Assert
            Assert.NotNull(result);
           
            Assert.Equal(gymClassId, result.Id);
            Assert.IsType<BusinessLogic.Entities.GymClass>(result);
            // Add more assertions based on your expected behavior
        }
        [Theory]
        [InlineData(1)]
        public void Any_ShouldReturnTrueForExistingId(int gymClassId)
        {
            // Act
            var result = sut.Any(gymClassId);

            // Assert
            Assert.True(result);
        }
    }

}


