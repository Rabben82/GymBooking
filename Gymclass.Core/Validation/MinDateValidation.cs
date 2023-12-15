using System.ComponentModel.DataAnnotations;

namespace GymClass.Core.Validation
{
    public class MinDateValidation : ValidationAttribute
    {
        private readonly DateTime minDateTime = DateTime.Now.AddDays(1);

        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime >= minDateTime;
            }
            return false;
        }
    }
}
