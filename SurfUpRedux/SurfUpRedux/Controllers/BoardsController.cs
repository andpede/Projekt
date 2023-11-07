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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, 
            string searchString, int? pageNumber)
        {
            // Identificerer bookinger, der er mere end 5 dage gamle
            var forældedeBookinger = _context.Booking
                .Where(b => b.EndDate < DateTime.Now.AddDays(-5));

            _context.Booking.RemoveRange(forældedeBookinger);
            await _context.SaveChangesAsync();

            ViewData["CurrentSort"] = sortOrder;

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? 
                "Name_desc" : "";

            ViewData["LengthSortParm"] = sortOrder == "Length" ? 
                "Length_desc" : "Length";

            ViewData["WidthSortParm"] = sortOrder == "Width" ? 
                "Width_desc" : "Width";

            ViewData["ThicknessSortParm"] = sortOrder == "Thickness" ? 
                "Thickness_desc" : "Thickness";

            ViewData["VolumeSortParm"] = sortOrder == "Volume" ? 
                "Volume_desc" : "Volume";

            ViewData["TypeSortParm"] = sortOrder == "Type" ? 
                "type_desc" : "Type";

            ViewData["PriceSortParm"] = sortOrder == "Price" ? 
                "Price_desc" : "Price";

            ViewData["EquipmentSortParm"] = sortOrder == "Equipment" ? 
                "Equipment_desc" : "Equipment";

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

            // Tjekker om brugeren er Admin eller Manager
            bool isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            if (!isAdminOrManager)
            {
                // Hvis brugeren ikke er Admin eller Manager, vis kun tilgængelige boards
                boards = boards.Where(b => b.IsAvailable);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                boards = boards.Where(b => b.Name.Contains(searchString) || 
                    b.Length.ToString().Contains(searchString) || 
                    b.Width.ToString().Contains(searchString) || 
                    b.Thickness.ToString().Contains(searchString) ||
                    b.Volume.ToString().Contains(searchString) || 
                    b.Type.Contains(searchString) || 
                    b.Price.ToString().Contains(searchString) || 
                    (b.Equipment != null && b.Equipment.Contains(searchString)));
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

                case "Width_desc":
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
                    boards = boards.OrderBy(b => b.Equipment != null ? 0 : 1)
                        .ThenBy(b => b.Equipment);
                    break;

                case "Equipment_desc":
                    boards = boards.OrderByDescending(b => b.Equipment != null ? 0 : 1)
                        .ThenBy(b => b.Equipment);
                    break;

                default:
                    boards = boards.OrderBy(b => b.Name);
                    break;
            }

            // Her sendes data til index view, som angiver om brugeren er Admin eller Manager
            ViewData["IsAdminOrManager"] = isAdminOrManager;

            int pageSize = 5;

            return View(await PaginatedList<Board>.
                CreateAsync(boards.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

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

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,ImageUrl")] Board board)
        {
            if (ModelState.IsValid)
            {
                _context.Add(board);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(board);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Hvis id er null, eller Board er ikke tilgængelig, returner 'NotFound'
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            // Hent boardet uden at spore det i context for at forbedre ydeevnen
            var board = await _context.Board
                .AsNoTracking() // Her anvendes AsNoTracking for at undgå tracking af entiteten
                .FirstOrDefaultAsync(m => m.Id == id);

            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,ImageUrl,RowVersion")] 
                Board board, byte[] rowVersion)
        {
            // Hvis board id ikke matcher det overførte id, returner 'NotFound'
            if (id != board.Id)
            {
                return NotFound();
            }

            // Sæt den originale RowVersion til den værdi, som blev sendt fra viewet
            _context.Entry(board).Property("RowVersion").OriginalValue = rowVersion;

            if (ModelState.IsValid)
            {
                try
                {
                    // Opdater boardet i context
                    _context.Update(board);
                    // Forsøg at gemme ændringerne i databasen
                    await _context.SaveChangesAsync();
                    // Hvis succesfuld, redirect til 'Index'
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single(); // Få den berørte entitet
                    var clientValues = (Board)exceptionEntry.Entity; // Værdier fra klienten
                    var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync(); // Hent nuværende værdier fra databasen

                    if (databaseEntry == null)
                    {
                        // Hvis databasen entry er null, er boardet blevet slettet af en anden bruger
                        ModelState.AddModelError(string.Empty, "Kan ikke gemme ændringerne. Boardet blev slettet af en anden bruger.");
                    }
                    else
                    {
                        // Hvis databasen har de nuværende værdier, sammenlign dem med klientens værdier
                        var databaseValues = (Board)databaseEntry.ToObject();

                        // Tilføj modelfejl for hver værdi der er forskellig fra databasens værdier
                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValues.Name}");
                        }
                        if (databaseValues.Length != clientValues.Length)
                        {
                            ModelState.AddModelError("Length", $"Nuværende værdi: {databaseValues.Length}");
                        }
                        if (databaseValues.Width != clientValues.Width)
                        {
                            ModelState.AddModelError("Width", $"Nuværende værdi: {databaseValues.Width}");
                        }
                        if (databaseValues.Thickness != clientValues.Thickness)
                        {
                            ModelState.AddModelError("Thickness", $"Nuværende værdi: {databaseValues.Thickness}");
                        }
                        if (databaseValues.Volume != clientValues.Volume)
                        {
                            ModelState.AddModelError("Volume", $"Nuværende værdi: {databaseValues.Volume}");
                        }
                        if (databaseValues.Type != clientValues.Type)
                        {
                            ModelState.AddModelError("Type", $"Nuværende værdi: {databaseValues.Type}");
                        }
                        if (databaseValues.Price != clientValues.Price)
                        {
                            ModelState.AddModelError("Price", $"Nuværende værdi: {databaseValues.Price}");
                        }
                        if (databaseValues.Equipment != clientValues.Equipment)
                        {
                            ModelState.AddModelError("Equipment", $"Nuværende værdi: {databaseValues.Equipment}");
                        }
                        if (databaseValues.ImageUrl != clientValues.ImageUrl)
                        {
                            ModelState.AddModelError("ImageUrl", $"Nuværende værdi: {databaseValues.ImageUrl}");
                        }

                        // Sæt RowVersion til den nye værdi hentet fra databasen
                        board.RowVersion = (byte[])databaseValues.RowVersion;
                        // Fjern den gamle RowVersion værdi fra ModelState
                        ModelState.Remove("RowVersion");

                        ModelState.AddModelError(string.Empty, "Det board du forsøgte at redigere "
                                + "er blevet ændret af en anden bruger efter du modtog de originale værdier. "
                                + "Redigeringsoperationen er blevet annulleret, og de nuværende værdier fra databasen "
                                + "er blevet vist. Hvis du stadig ønsker at redigere dette board, klik "
                                + "'Gem' knappen igen. Ellers klik 'Tilbage til liste' linket.");
                    }
                }
                catch (Exception ex)
                {
                    // Håndter andre mulige fejl og vis en fejlmeddelelse
                    ModelState.AddModelError(string.Empty, "Kan ikke gemme ændringer. Prøv igen, og hvis problemet vedvarer, "
                        + "kontakt systemadministratoren.");
                }
            }

            // Hvis vi når her, er der sket en fejl, så vi viser formen igen med fejlene
            return View(board);
        }


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
