using GymClass.Data.Repositories;
using GymClass.Tests.Repositories.Fixtures;

namespace GymClass.Tests.Repositories
{
    public class GymClassRepositoryTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture fixture;
        private readonly GymClassRepository sut;

        public GymClassRepositoryTests(TestDatabaseFixture fixture)
        {
            this.fixture = fixture;

            sut = new GymClassRepository(fixture.Context, fixture.HttpContextAccessor);
        }
        [Fact]
        public async Task GetAsync_ShouldReturnGymClass()
        {
            //Arrange

            //Act
            var result = await sut.GetAsync(userId: "expectedUserId", showHistory:true);
            //Assert
            Assert.IsType<List<Core.Entities.GymClass>>(result);
        }

        [Theory]
        [InlineData(1)]
        public async Task BookingToggle_ShouldToggleBooking(int gymClassId)
        {
            //Arrange

            // Act
            var result = await sut.BookingToggleAsync(gymClassId);

            // Assert
            Assert.NotNull(result);
           
            Assert.Equal(gymClassId, result.Id);
            Assert.IsType<Core.Entities.GymClass>(result);
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


