using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfUpReduxAPI.Data;
using SurfUpReduxAPI.Models;

namespace SurfUpReduxAPI.Controllers
{
    [Route("api/Board")]
    [ApiController]

    public class BookingsController : ControllerBase
    {
        private readonly SurfUpReduxContext _context;
        //private readonly UserManager<SurfUpUser> _userManager;

        public BookingsController(SurfUpReduxContext context)
        {
            _context = context;

        }
        //I like BBC Big bad crossaints
        // GET: api/Boards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetAllBoards()
        {

            var boards = from b in _context.Board
                         select b;

            foreach (var board in boards)
            {
                // Assume the board is initially available
                board.IsAvailable = true;

                var boardBookings = _context.Booking.Where(booking => booking.BoardId == board.Id);

                foreach (var booking in boardBookings)
                {
                    if (booking.StartDate < DateTime.Now && booking.EndDate > DateTime.Now)
                    {
                        // The board is booked for the current time
                        board.IsAvailable = false;
                        break; // No need to check other bookings
                    }
                }

                // Update the board's availability status in the database
                _context.Update(board);
            }

            await _context.SaveChangesAsync();

            boards = from b in boards
                     where (b.IsAvailable == true)
                     select b;

            return await boards.ToListAsync<Board>();
        }










        //// GET: Bookings/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Booking == null)
        //    {
        //        return NotFound();
        //    }

        //    var booking = await _context.Booking
        //        .Include(b => b.Board)         
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (booking == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(booking);
        //}

        //    // GET: Bookings/Create
        //    public async Task<IActionResult> Create(int boardId)
        //    {



        //        if (User.IsInRole("Admin") || User.IsInRole("Manager"))
        //        {
        //            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name");
        //            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
        //        }
        //        else
        //        {

        //            var user = await _userManager.GetUserAsync(User);
        //            ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email");


        //            var board = await _context.Board.FindAsync(boardId);
        //            if (board != null)
        //            {
        //                ViewData["BoardId"] = new SelectList(new List<Board> { board }, "Id", "Name");
        //            }
        //            else
        //            {

        //                return NotFound();
        //            }

        //        }

        //        return View();
        //    }

        //    // POST: Bookings/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId")] Booking booking)
        //    {
        //        ModelState.Remove("Board");
        //        ModelState.Remove("User");


        //        foreach (var modelState in ModelState.Values)
        //        {
        //            foreach (var error in modelState.Errors)
        //            {
        //                Console.WriteLine(error.ErrorMessage);
        //            }
        //        }

        //        if (booking.StartDate > booking.EndDate)
        //        {
        //            ModelState.AddModelError("StartDate", "Startdate has to be before enddate");
        //        }
        //        if (booking.EndDate >= booking.StartDate.AddDays(3))
        //        {
        //            ModelState.AddModelError("Enddate", "You can only book this board for 3 days");
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(booking);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));

        //        }

        //        ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);

        //        if (User.IsInRole("Admin") || User.IsInRole("Manager"))
        //        {
        //            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
        //        }
        //        else if (User.IsInRole("User"))
        //        {
        //            var user = await _userManager.GetUserAsync(User);
        //            ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email", booking.UserId);
        //        }




        //        return View(booking);
        //    }



        //    // GET: Bookings/Edit/5
        //    [Authorize(Roles = "Admin,Manager")]
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null || _context.Booking == null)
        //        {
        //            return NotFound();
        //        }

        //        var booking = await _context.Booking.FindAsync(id);
        //        if (booking == null)
        //        {
        //            return NotFound();
        //        }
        //        ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
        //        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
        //        return View(booking);
        //    }



        //    // POST: Bookings/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [Authorize(Roles = "Admin,Manager")]
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,BoardId,UserId")] Booking booking)
        //    {
        //        if (id != booking.Id)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(booking);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!BookingExists(booking.Id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
        //        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
        //        return View(booking);
        //    }


        //    // GET: Bookings/Delete/5
        //    [Authorize(Roles = "Admin,Manager,User")]
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null || _context.Booking == null)
        //        {
        //            return NotFound();
        //        }

        //        var booking = await _context.Booking
        //            .Include(b => b.Board)
        //            .Include(b => b.User)
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (booking == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(booking);
        //    }


        //    // POST: Bookings/Delete/5
        //    [Authorize(Roles = "Admin,Manager,User")]
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        if (_context.Booking == null)
        //        {
        //            return Problem("Entity set 'SurfUpReduxContext.Bookings'  is null.");
        //        }
        //        var booking = await _context.Booking.FindAsync(id);
        //        if (booking != null)
        //        {
        //            _context.Booking.Remove(booking);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool BookingExists(int id)
        //    {
        //        return (_context.Booking?.Any(e => e.Id == id)).GetValueOrDefault();
        //    }
        //}
    }
}
