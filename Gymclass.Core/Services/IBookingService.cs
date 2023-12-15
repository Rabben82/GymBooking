using System.Security.Claims;

namespace GymClass.Core.Services;

public interface IBookingService
{
    Task<bool> IsMemberBooked(ClaimsPrincipal user, int gymClassId);
}