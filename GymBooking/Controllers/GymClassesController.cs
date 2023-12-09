using GymClass.BusinessLogic.Entities;
using GymClass.BusinessLogic.Repositories;
using GymClass.BusinessLogic.Services;
using GymClass.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GymBooking.WebApp.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMessageToUserService messageToUserService;
        private readonly IGymClassRepository gymClassRepository;
        // private IQueryable<GymClass> getClasses;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMessageToUserService messageToUserService, IGymClassRepository gymClassRepository)
        {
            _context = context;
            this.userManager = userManager;
            this.messageToUserService = messageToUserService;
            this.gymClassRepository = gymClassRepository;
        }
        [AllowAnonymous]
        // GET: GymClasses
        public async Task<IActionResult> Index(string userId, bool showHistory = false, bool showBooked = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) NotFound("UserId not found");

            return View(await gymClassRepository.GetAsync(userId, showHistory, showBooked));
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id, string message ="DETAILS")
        {
            if (id == null)
            {
                return NotFound("Id is not found");
            }

            return View(await gymClassRepository.GetAsync((int)id, message));
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            messageToUserService.AddMessage("CREATE NEW CLASS");

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
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id, string message="EDIT GYM CLASS")
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(await gymClassRepository.GetAsync((int)id, message));
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
                    gymClassRepository.Update(gymClass);
                    await _context.SaveChangesAsync();
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
                return NotFound();
            }

            return View(await gymClassRepository.GetAsync((int)id, message));
        }

        // POST: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await gymClassRepository.GetAsync(id);
            
            gymClassRepository.Remove(gymClass);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return BadRequest("Id of the gym class not found");

            var gymClass = await _context.GymClasses
                .Include(m => m.AttendingMembers)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gymClass == null) return NotFound("Gym class not found");

            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser == null) return NotFound("No User Found");

            //Is the user already attending
            var attendingMember = gymClass.AttendingMembers
                .FirstOrDefault(member => member.ApplicationUserId == currentUser.Id);

            if (attendingMember == null)
            {
                gymClass.AttendingMembers.Add(new ApplicationUserGymClass
                {
                    ApplicationUserId = currentUser.Id,
                    GymClassId = gymClass.Id

                });
            }
            else
            {
                gymClass.AttendingMembers.Remove(attendingMember);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
