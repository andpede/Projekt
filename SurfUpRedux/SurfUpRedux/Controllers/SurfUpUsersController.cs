using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpRedux.Data;
using SurfUpRedux.Models;

namespace SurfUpRedux.Controllers
{
    public class SurfUpUsersController : Controller
    {
        private readonly SurfUpReduxContext _context;
        private readonly UserManager<SurfUpUser> _userManager;

        public SurfUpUsersController(SurfUpReduxContext context, UserManager<SurfUpUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(string userId)
        {
            if (userId == null || _context.Users == null)
            {
                return NotFound();
            }

            var surfUpUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (surfUpUser == null)
            {
                return NotFound();
            }

            return View(surfUpUser);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'YourDbContext.Users' is null.");
            }

            var surfUpUser = await _context.Users.FindAsync(userId);

            if (surfUpUser != null)
            {
                _context.Users.Remove(surfUpUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}





