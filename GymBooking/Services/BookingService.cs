﻿using GymBooking.Data;
using GymBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public BookingService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<bool> IsMemberBooked(string userId, int gymClassId)
        {
            var isBooked = await context.GymClasses
                .AnyAsync(g => g.Id == gymClassId && g.AttendingMembers.Any(m => m.ApplicationUserId == userId));
            return isBooked;
        }
    }
}