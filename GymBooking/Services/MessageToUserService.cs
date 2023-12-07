namespace GymBooking.Services
{
    public class MessageToUserService : IMessageToUserService
    {
        private string Message { get; set; } = string.Empty;

        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("The message can't be null or empty or consist only of white spaces", nameof(message));
            }

            Message = message;
        }

        public string ShowMessage()
        {
            return Message;
        }
    }
}
