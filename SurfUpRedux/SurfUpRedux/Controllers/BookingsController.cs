using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        // GET: Bookings
        public async Task<IActionResult> Index()
        {

            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Admin") || (User.IsInRole("Manager")))
            {
                var surfUpReduxContext = _context.Booking.Include(b => b.Board).Include(b => b.User);

                return View(await surfUpReduxContext.ToListAsync());
            }
            else /*if (User.IsInRole("User")) */
            {
                var surfUpReduxContext = _context.Booking.Include(b => b.Board)
                                                         .Include(b => b.User)
                                                         .Where(b => b.UserId == userId);
                return View(await surfUpReduxContext.ToListAsync());
            }
            
        }

        // GET: Bookings/Details/5
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

        // GET: Bookings/Create
        //public async Task<IActionResult> Create(int boardId)
        //{

        //    //var userId = _userManager.GetUserId(User);

        //    //{
        //    //    booking.UserId = userId;
        //    //    booking.BoardId = boardId;
        //    //    booking.BookingS = DateTime.Now;
        //    //    booking.BookingEnd = DateTime.Now;
        //    //};
        //    //return View(booking);

        //    if (User.IsInRole("Admin") || User.IsInRole("Manager"))
        //    {
        //        ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name");
        //        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");

        //    }
        //    else
        //    {

        //        var user = await _userManager.GetUserAsync(User);
        //        ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email");


        //        var board = await _context.Board.FindAsync(boardId);
        //        if (board != null)
        //        {
        //            ViewData["BoardId"] = new SelectList(new List<Board> { board }, "Id", "Name");
        //        }
        //        else
        //        {

        //            return NotFound(); 
        //        }   

        //    }

        //    return View();
        //}

        //// POST: Bookings/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId")] Booking booking)
        //{
        //    ModelState.Remove("Board");
        //    ModelState.Remove("User");

        //    foreach (var modelState in ModelState.Values)
        //    {
        //        foreach (var error in modelState.Errors)
        //        {
        //            Console.WriteLine(error.ErrorMessage);
        //        }
        //    }

        //    if (booking.StartDate > booking.EndDate)
        //    {
        //        ModelState.AddModelError("StartDate", "Startdate has to be before enddate");
        //    }
        //    if (booking.EndDate >= booking.StartDate.AddDays(3))
        //    {
        //        ModelState.AddModelError("Enddate", "You can only book this board for 3 days");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(booking);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CurrentRowVersion"] = booking.RowVersion;

        //    ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);

        //    if (User.IsInRole("Admin") || User.IsInRole("Manager")) 
        //    {
        //        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
        //    }
        //    else if (User.IsInRole("User"))
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email", booking.UserId);
        //    }
        //    return View(booking);
        //}
        // GET: Bookings/Create
        public async Task<IActionResult> Create(int boardId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name");
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email");

                var board = await _context.Board.FindAsync(boardId);
                if (board != null)
                {
                    ViewData["BoardId"] = new SelectList(new List<Board> { board }, "Id", "Name");
                }
                else
                {
                    return NotFound();
                }
            }

            // Create a new Booking with a new RowVersion
            var booking = new Booking
            {
                RowVersion = new byte[8], // Generate a new RowVersion
                StartDate = DateTime.Now, // Set StartDate to the current date
                EndDate = DateTime.Now
            };

            return View(booking);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId,RowVersion")] Booking booking)
        {
            ModelState.Remove("Board");
            ModelState.Remove("User");

            if (booking.StartDate > booking.EndDate)
            {
                ModelState.AddModelError("StartDate", "Start date has to be before end date");
            }
            if (booking.EndDate >= booking.StartDate.AddDays(3))
            {
                ModelState.AddModelError("EndDate", "You can only book this board for 3 days");
            }

            if (ModelState.IsValid)
            {
                // Generate a new RowVersion for the new booking
                booking.RowVersion = new byte[8];

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            }
            else if (User.IsInRole("User"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email", booking.UserId);
            }
            return View(booking);
        }


        // GET: Bookings/Edit/5
        //[Authorize(Roles = "Admin,Manager")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Booking == null)
        //    {
        //        return NotFound();
        //    }

        //    var booking = await _context.Booking.FindAsync(id);
        //    if (booking == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
        //    return View(booking);
        //}

        //// POST: Bookings/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin,Manager")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,BoardId,UserId")] Booking booking)
        //{
        //    if (id != booking.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(booking);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BookingExists(booking.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
        //    return View(booking);
        //}
        // GET: Bookings/Edit/5
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

            // Pass the current row version to the view
            ViewData["CurrentRowVersion"] = booking.RowVersion;

            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
      
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,BoardId,UserId,RowVersion")] Booking booking, byte[] currentRowVersion)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check for concurrency conflict
                if (!currentRowVersion.SequenceEqual(booking.RowVersion))
                {
                    // Handle the concurrency conflict here, e.g., by displaying an error message
                    ModelState.AddModelError("Concurrency", "Concurrency conflict detected. Another user has updated this booking.");
                    ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
                    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
                    return View(booking);
                }

                // If there's no concurrency conflict, proceed with the update
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return View(booking);
        }




        // GET: Bookings/Delete/5
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


        // POST: Bookings/Delete/5
        [Authorize(Roles = "Admin,Manager,User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'SurfUpReduxContext.Bookings'  is null.");
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
          return (_context.Booking?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
