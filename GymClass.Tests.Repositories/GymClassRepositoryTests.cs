using GymClass.Data.Repositories;
using GymClass.BusinessLogic.Exceptions;
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
            Assert.IsType<List<BusinessLogic.Entities.GymClass>>(result);
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
            Assert.IsType<BusinessLogic.Entities.GymClass>(result);
            // Add more assertions based on your expected behavior
        }
        [Theory]
        [InlineData(0)]
        public async Task BookingToggle_ShouldReturnEntityNotFound(int gymClassId)
        {
            //Act && Assert
            //This is an asynchronous assertion using xUnit's Assert.ThrowsAsync method.
            //It is used to assert that the provided asynchronous code (inside the lambda expression)
            //throws a specific exception, in this case, EntityNotFoundException.

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                var result = await sut.BookingToggleAsync(gymClassId);
            });
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


