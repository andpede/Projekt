using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SurfUpReduxAPI.Data;
using SurfUpReduxAPI.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SurfUpReduxAPI.Controllers.v2
{
    [Route("api/v{version:ApiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]

    public class BookingControllerV2 : ControllerBase
    {
        private readonly SurfUpReduxContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HttpClient _client;

        public BookingControllerV2(SurfUpReduxContext context, UserManager<IdentityUser> userManager, HttpClient client)
        {
            _context = context;
            _userManager = userManager;
            _client = client;
        }




        }
    }