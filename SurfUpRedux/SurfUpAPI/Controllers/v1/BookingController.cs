using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SurfUpReduxAPI.Data;
using SurfUpReduxAPI.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SurfUpReduxAPI.Controllers.v1
{
    [Route("api/v{version:ApiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class BookingController : ControllerBase
    {
        private readonly SurfUpReduxContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HttpClient _client;

        public BookingController(SurfUpReduxContext context, UserManager<IdentityUser> userManager, HttpClient client)
        {
            _context = context;
            _userManager = userManager;
            _client = client;
        }



        //Create bookings
        [HttpPost("Book")]

        public async Task<IActionResult> Create([Bind("StartDate,EndDate,BoardId,UserId")] Booking booking)
        {
            ModelState.Remove("Board");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
             
                _context.Booking.Add(booking);
                var board = await _context.Board.FindAsync(booking.BoardId);
                if (board != null)
                {
                    board.IsAvailable = false;
                    _context.Update(board);
                }

                await _context.SaveChangesAsync();

                return Ok(nameof(Index));
            }

            return BadRequest();
        }


            //[HttpPost("Book")]
            //public async Task<ActionResult<Board>> Book([FromBody] Booking booking)
            //{

            //        if (_context.Board == null)
            //        {
            //            return Problem("Entity set 'SurfUpReduxContext.Board' is null");
            //        }

            //       // Booking createBooking = new Booking(booking.StartDate, booking.EndDate, booking.BoardId, booking.UserId);
            //        //_context.Booking.Add(createBooking);
            //        //await _context.SaveChangesAsync();


            //    var boards = await _context.Board.FindAsync(booking.BoardId);
            //    if (boards == null)
            //    {
            //        return NotFound();
            //    }

            //    boards.bookings = new List<Booking>();

            //    boards.bookings.Add(booking);



            //    await _context.SaveChangesAsync();

            //    return Ok();

            //}

        }
    }