using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SurfUpRedux.Data;
using SurfUpRedux.Models;

namespace SurfUpRedux.Controllers
{
    public class BoardsController : Controller
    {
        private readonly SurfUpReduxContext _context;

        public BoardsController(SurfUpReduxContext context)
        {
            _context = context;
        }

        // GET: Boards - Modificeret med søgefunktion
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_desc" : "Length";
            ViewData["WidthSortParm"] = sortOrder == "Width" ? "Width_desc" : "Width";
            ViewData["ThicknessSortParm"] = sortOrder == "Thickness" ? "Thickness_desc" : "Thickness";
            ViewData["VolumeSortParm"] = sortOrder == "Volume" ? "Volume_desc" : "Volume";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "Price_desc" : "Price";
            ViewData["EquipmentSortParm"] = sortOrder == "Equipment" ? "Equipment_desc" : "Equipment";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var boards = from b in _context.Board
                         select b;
            if (!String.IsNullOrEmpty(searchString))
            {
                boards = boards.Where(b => b.Name.Contains(searchString) || b.Length.ToString().Contains(searchString)
                || b.Width.ToString().Contains(searchString) || b.Thickness.ToString().Contains(searchString) ||
                b.Volume.ToString().Contains(searchString) || b.Type.Contains(searchString) ||
                b.Price.ToString().Contains(searchString) || (b.Equipment != null && b.Equipment.Contains(searchString)));
            }
            switch (sortOrder)
            {
                case "Name_desc":
                    boards = boards.OrderByDescending(b => b.Name);
                    break;
                case "Length":
                    boards = boards.OrderBy(b => b.Length);
                    break;
                case "Length_desc":
                    boards = boards.OrderByDescending(b => b.Length);
                    break;
                case "Width":
                    boards = boards.OrderBy(b => b.Width);
                    break;
                case "Widt_desc":
                    boards = boards.OrderByDescending(b => b.Width);
                    break;
                case "Thickness":
                    boards = boards.OrderBy(b => b.Thickness);
                    break;
                case "Thickness_desc":
                    boards = boards.OrderByDescending(b => b.Thickness);
                    break;
                case "Volume":
                    boards = boards.OrderBy(b => b.Volume);
                    break;
                case "Volume_desc":
                    boards = boards.OrderByDescending(b => b.Volume);
                    break;
                case "Type":
                    boards = boards.OrderBy(b => b.Type);
                    break;
                case "Type_desc":
                    boards = boards.OrderByDescending(b => b.Type);
                    break;
                case "Price":
                    boards = boards.OrderBy(b => b.Price);
                    break;
                case "Price_desc":
                    boards = boards.OrderByDescending(b => b.Price);
                    break;
                case "Equipment":
                    boards = boards.OrderBy(b => b.Equipment != null ? 0 : 1).ThenBy(b => b.Equipment);
                    break;
                case "Equipment_desc":
                    boards = boards.OrderByDescending(b => b.Equipment != null ? 0 : 1).ThenBy(b => b.Equipment);
                    break;
                default:
                    boards = boards.OrderBy(b => b.Name);
                    break;
            }
            int pageSize = 5;

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



            return View(await PaginatedList<Board>.CreateAsync(boards.AsNoTracking(), pageNumber ?? 1, pageSize));

           
        }
     
        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // GET: Boards/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,ImageUrl")] Board board)
        {
            if (ModelState.IsValid)
            {
                _context.Add(board);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(board);
        }

        // GET: Boards/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            return View(board);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,ImageUrl")] Board board)
        {
            if (id != board.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(board);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(board.Id))
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
            return View(board);
        }

        // GET: Boards/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Board == null)
            {
                return Problem("Entity set 'SurfUpReduxContext.Board'  is null.");
            }
            var board = await _context.Board.FindAsync(id);
            if (board != null)
            {
                _context.Board.Remove(board);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
          return (_context.Board?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
