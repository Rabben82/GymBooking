using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymClass.BusinessLogic.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters long")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters long")]
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public DateTime TimeOfRegistration { get; set; }

        //Navigation Property
        public ICollection<ApplicationUserGymClass> AttendingClasses { get; set; } =
            new List<ApplicationUserGymClass>();
    }
}
