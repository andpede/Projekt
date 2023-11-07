using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SurfUpRedux.Data;
using SurfUpRedux.Models;
using SurfUpRedux.Models.ViewModel;

namespace SurfUpRedux.Controllers
{
    public class BookingsController : Controller
    {
        private readonly SurfUpReduxContext _context;
        private readonly UserManager<SurfUpUser> _userManager;

        //Uri baseAddress = new Uri($"https://localhost:7203/api/v1.0/Booking/Book");
        private readonly HttpClient _client;

        public BookingsController(SurfUpReduxContext context, UserManager<SurfUpUser> userManager, HttpClient client)
        {
            _context = context;
            _userManager = userManager;
            _client = client;
            //_client.BaseAddress = baseAddress;  
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
            else 
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId")] Booking booking)
        {            

            var UserId = _userManager.GetUserId(User);
            string URL = $"https://localhost:7203/api/v1.0/Booking/Book";
            HttpClient client = new HttpClient();


            booking.UserId = UserId;
            booking.StartDate = DateTime.Now;
            

            await client.PostAsJsonAsync(URL, booking);

            return RedirectToAction(nameof(Index));
        

        }

        //[Authorize(Roles = "Admin,Manager,User")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId")] Booking booking)
        //{
        //    ModelState.Remove("Board");
        //    ModelState.Remove("User");

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(booking);

        //        var board = await _context.Board.FindAsync(booking.BoardId);
        //        if (board != null)
        //        {
        //            board.IsAvailable = false;
        //            _context.Update(board);
        //        }

        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Ved fejl, genopret SelectList for BoardId og UserId afhÃ¦ngigt af brugerrollen.
        //    ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);

        //    if (User.IsInRole("Admin") || User.IsInRole("Manager"))
        //    {
        //        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
        //    }
        //    else if (User.IsInRole("User"))
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        ViewData["UserId"] = new SelectList(new List<SurfUpUser> { user }, "Id", "Email", user.Id);
        //    }

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
            ViewData["BoardId"] = new SelectList(_context.Board, "Id", "Name", booking.BoardId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return View(booking);
        }



        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,BoardId,UserId")] Booking booking)
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
