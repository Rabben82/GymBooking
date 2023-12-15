using GymClass.BusinessLogic.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GymBooking.WebApp.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly IUnitOfWork uow;

        public GymClassesController(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string userId, string pageName, bool showHistory = false, bool showBooked = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) NotFound("User not found");

            ViewData["Title"] = pageName;

            return View(await uow.GymClassRepository.GetAsync(userId, showHistory, showBooked));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !GymClassExists((int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(await uow.GymClassRepository.GetAsync((int)id));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
           // uow.GymClassRepository.AddMessageToUser(PageNameCreate);

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass.BusinessLogic.Entities.GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                uow.GymClassRepository.Add(gymClass);
                await uow.SaveCompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !GymClassExists((int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(await uow.GymClassRepository.GetAsync((int)id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass.BusinessLogic.Entities.GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    uow.GymClassRepository.Update(gymClass);
                    await uow.SaveCompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(await uow.GymClassRepository.GetAsync((int)id));
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var gymClass = await uow.GymClassRepository.GetAsync((int)id);

            if (gymClass == null)
            {
                return RedirectToAction(nameof(Index));
            }

            uow.GymClassRepository.Remove(gymClass);

            await uow.SaveCompleteAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        private bool GymClassExists(int id)
        {
            return uow.GymClassRepository.Any(id);
        }
        [HttpGet]
        public async Task<IActionResult> BookingToggle(int? id, string currentUrl)
        {
            if (id == null) return NotFound("No Gym class Found");

            await uow.GymClassRepository.BookingToggleAsync(id);

            await uow.SaveCompleteAsync();

            // Redirect back to the referring URL
            return Redirect(currentUrl);
        }
        [HttpGet]
        public async Task<IActionResult> MyBookingHistory()
        {
            var myBookings = await uow.GymClassRepository.MyBookingHistoryAsync();

            return View(myBookings);
        }
    }
}
