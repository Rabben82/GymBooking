namespace GymClass.BusinessLogic.Services;

public interface IBookingService
{
    Task<bool> IsMemberBooked(string userId, int gymClassId);
}