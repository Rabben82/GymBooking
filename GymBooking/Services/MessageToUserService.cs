using GymClass.BusinessLogic.Services;

namespace GymBooking.WebApp.Services
{
    public class MessageToUserService : IMessageToUserService
    {
        private string Message { get; set; } = string.Empty;

        public void AddMessage(string message)
        {
            Message = message;
        }

        public string ShowMessage()
        {
            return Message;
        }
    }
}
