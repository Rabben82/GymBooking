namespace GymBooking.Services;

public interface IMessageToUserService
{
    void AddMessage(string message);
    string ShowMessage();
}