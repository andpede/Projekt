using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfUpRedux.Data;
using SurfUpRedux.Models;

namespace SurfUpRedux.Controllers
{
    public class BookingsController : Controller
    {
        private readonly SurfUpReduxContext _context;
        private readonly UserManager<SurfUpUser> _userManager;

        public BookingsController(SurfUpReduxContext context, UserManager<SurfUpUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Index()
        {
            // Identificerer bookinger, der er mere end 5 dage gamle
            var forældedeBookinger = _context.Booking
                .Where(b => b.EndDate < DateTime.Now.AddDays(-5));

            _context.Booking.RemoveRange(forældedeBookinger);

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin") || (User.IsInRole("Manager")))
            {
                var surfUpReduxContext = _context.Booking.Include(b => b.Board).Include(b => b.User);

                return View(await surfUpReduxContext.ToListAsync());
            }
            else if (User.IsInRole("User"))
            {
                var surfUpReduxContext = _context.Booking.Include(b => b.Board)
                                                         .Include(b => b.User)
                                                         .Where(b => b.UserId == userId);

                return View(await surfUpReduxContext.ToListAsync());
            }
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Board)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Create(int boardId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                var board = await _context.Board.FindAsync(boardId);
                if (board == null || !board.IsAvailable)
                {
                    return NotFound();
                }
                ViewData["BoardId"] = new SelectList(new List<Board> { board }, "Id", "Name");
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else if (User.IsInRole("User"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email");

                var board = await _context.Board.FindAsync(boardId);
                if (board == null || !board.IsAvailable)
                {
                    return NotFound();
                }
                ViewData["BoardId"] = new SelectList(new List<Board> { board }, "Id", "Name");
            }

            return View();
        }



        [Authorize(Roles = "Admin,Manager,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId")] Booking booking)
        {
            ModelState.Remove("Board");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {             
                if (booking.StartDate >= booking.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Slutdato skal være efter startdatoen.");
                }

                if (booking.StartDate < DateTime.Today)
                {
                    ModelState.AddModelError("StartDate", "Booking skal være fra nu af og frem.");
                }

                TimeSpan duration = booking.EndDate - booking.StartDate;
                if (duration.TotalDays > 5)
                {
                    ModelState.AddModelError("EndDate", "Maximum booking er på 5 dage.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(booking);

                    var board = await _context.Board.FindAsync(booking.BoardId);
                    if (board != null)
                    {
                        board.IsAvailable = false;
                        _context.Update(board);
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            // Ved fejl, genopret SelectList for BoardId og UserId afhængigt af brugerrollen.
            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            }
            else if (User.IsInRole("User"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email", user.Id);
            }

            return View(booking);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);

            return View(booking);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("Id,StartDate,EndDate,BoardId,UserId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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

            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);

            return View(booking);
        }

        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Board)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'SurfUpReduxContext.Bookings' er null.");
            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking != null)
            {
                // Hent den tilknyttede bræt.
                var board = await _context.Board.FindAsync(booking.BoardId);

                if (board != null)
                {
                    // Sæt IsAvailable-egenskaben for brættet til true.
                    board.IsAvailable = true;
                    _context.Update(board);
                }

                // Fjern bookingen.
                _context.Booking.Remove(booking);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool BookingExists(int id)
        {
          return (_context.Booking?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
