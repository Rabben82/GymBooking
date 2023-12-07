﻿using System.Collections;
using System.ComponentModel.DataAnnotations;
using GymBooking.Validation;

namespace GymBooking.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name field is required")]
        [StringLength(30, MinimumLength = 2 , ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd-HH:mm}", ApplyFormatInEditMode = true)]
        [MinDateValidation(ErrorMessage = "Start time must be at least 1 day after today.")]
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd-HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime => StartTime + Duration;
        public string Description { get; set; } = string.Empty;
        public string FormattedDurationWithoutSeconds => $"{(int)Duration.TotalHours:D2}:{Duration.Minutes:D2}";
        //Navigation Property
        public ICollection<ApplicationUserGymClass> AttendingMembers{ get; set; } = new List<ApplicationUserGymClass>();
    }
}
