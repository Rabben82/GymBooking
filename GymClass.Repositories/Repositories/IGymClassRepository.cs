namespace GymClass.Repositories.Repositories;

public interface IGymClassRepository
{
    Task<List<BusinessLogic.Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false);
}