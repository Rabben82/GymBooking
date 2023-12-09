namespace GymClass.BusinessLogic.Repositories;

public interface IGymClassRepository
{
    Task<List<BusinessLogic.Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false);
    Task<BusinessLogic.Entities.GymClass> GetAsync(int id, string message="");
    public void Remove(BusinessLogic.Entities.GymClass gymClass);
    public void Update(BusinessLogic.Entities.GymClass gymClass);
}