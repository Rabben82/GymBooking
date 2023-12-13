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
        private string pageName = string.Empty;
        private const string PageNameDetails = "DETAILS";
        private const string PageNameCreate = "CREATE NEW CLASS";
        private const string PageNameEdit = "EDIT GYM CLASS";
        private const string PageNameDelete = "DELETE THIS CLASS?";
        private const string PageNameBookingHistory = "My Booking History";

        public GymClassesController(IUnitOfWork uow)
        {
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !GymClassExists((int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            pageName = PageNameDetails;

            return View(await uow.GymClassRepository.GetAsync((int)id, pageName));
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            uow.GymClassRepository.AddMessageToUser(PageNameCreate);

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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !GymClassExists((int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            pageName = PageNameEdit;

            return View(await uow.GymClassRepository.GetAsync((int)id, pageName));
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            pageName = PageNameDelete;

            return View(await uow.GymClassRepository.GetAsync((int)id, pageName));
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

        public async Task<IActionResult> BookingToggle(int? id, string currentUrl)
        {
            if (id == null) return NotFound("No Gym class Found");

            await uow.GymClassRepository.BookingToggle(id);

            await uow.SaveCompleteAsync();

            // Redirect back to the referring URL
            return Redirect(currentUrl);
        }
        public async Task<IActionResult> MyBookingHistory()
        {
            pageName = PageNameBookingHistory;

            var myBookings = await uow.GymClassRepository.MyBookingHistory(pageName);

            return View(myBookings);
        }
    }
}
