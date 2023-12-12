namespace GymClass.BusinessLogic.Services;

public interface IBookingService
{
    Task<bool> IsMemberBooked(int gymClassId);
}