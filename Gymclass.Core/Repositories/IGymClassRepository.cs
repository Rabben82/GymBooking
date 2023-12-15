namespace GymClass.Core.Repositories;

public interface IGymClassRepository
{
    public Task<List<Core.Entities.GymClass>> GetAsync(string userId, bool showHistory = false, bool showBooked = false);
    public Task<Core.Entities.GymClass> GetAsync(int id);
    public void Remove(Core.Entities.GymClass gymClass);
    public void Update(Core.Entities.GymClass gymClass);
    public void Add(Core.Entities.GymClass gymClass);
    public Task<Core.Entities.GymClass> BookingToggleAsync(int? id);
    public bool Any(int? id);
    public Task<IList<Core.Entities.GymClass>> MyBookingHistoryAsync();
}