namespace GymBooking.Services;

public interface IBookingService
{
    Task<bool> IsMemberBooked(string userId, int gymClassId);
}