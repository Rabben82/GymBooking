
namespace GymClass.BusinessLogic.Entities
{
    public class ApplicationUserGymClass
    {
        //Foreign Keys
        public string? ApplicationUserId { get; set; }
        public int GymClassId { get; set; }

        //Navigation Properties
        public ApplicationUser ApplicationUser { get; set; } = default!;
        public GymClass GymClass { get; set; } = default!;

    }
}
