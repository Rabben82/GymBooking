using GymClass.BusinessLogic.Entities;
using GymClass.BusinessLogic.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GymBooking.WebApp.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWork uow;

        public GymClassesController(UserManager<ApplicationUser> userManager, IUnitOfWork uow)
        {
            this.userManager = userManager;
            this.uow = uow;
        }
        [AllowAnonymous]
        // GET: GymClasses
        public async Task<IActionResult> Index(string userId, bool showHistory = false, bool showBooked = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) NotFound("User not found");

            return View(await uow.GymClassRepository.GetAsync(userId, showHistory, showBooked));
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id, string message ="DETAILS")
        {
            if (id == null || !GymClassExists((int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(await uow.GymClassRepository.GetAsync((int)id, message));
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            uow.GymClassRepository.AddMessageToUser("CREATE NEW CLASS");

            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id, string message="EDIT GYM CLASS")
        {
            if (id == null || !GymClassExists((int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(await uow.GymClassRepository.GetAsync((int)id, message));
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, string message ="DELETE THIS CLASS?")
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(await uow.GymClassRepository.GetAsync((int)id, message));
        }

        // POST: GymClasses/Delete/5
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

        private bool GymClassExists(int id)
        {
            return uow.GymClassRepository.Any(id);
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser == null) return NotFound("No User Found");

            await uow.GymClassRepository.BookingToggle(id, currentUser);

            await uow.SaveCompleteAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MyBookingHistory(string message ="My Booking History")
        {
            var currentUser = userManager.GetUserId(User);

            if (currentUser == null) return NotFound("No User Found");

            var myBookings = await uow.GymClassRepository.MyBookingHistory(currentUser, message);

            return View(myBookings);
        }
    }
}
    