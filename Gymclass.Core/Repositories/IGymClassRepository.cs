using GymClass.BusinessLogic.Entities;

namespace GymClass.BusinessLogic.Repositories;

public interface IGymClassRepository
{
    public Task<List<Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false);
    public Task<Entities.GymClass> GetAsync(int id, string pageName="");
    public void Remove(Entities.GymClass gymClass);
    public void Update(Entities.GymClass gymClass);
    public void Add(Entities.GymClass gymClass);
    public Task<Entities.GymClass> BookingToggleAsync(int? id);
    public void AddMessageToUser(string message);
    public bool Any(int? id);
    public Task<IList<BusinessLogic.Entities.GymClass>> MyBookingHistoryAsync(string pageName);
}