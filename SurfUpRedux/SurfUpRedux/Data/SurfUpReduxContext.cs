using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurfUpRedux.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SurfUpRedux.Data
{
    public class SurfUpReduxContext : IdentityDbContext<SurfUpUser>
    {
        public SurfUpReduxContext (DbContextOptions<SurfUpReduxContext> options)
            : base(options)
        {
        }

        public DbSet<Board> Boards { get; set; } = default!;
        public DbSet<Booking> Bookings { get; set; } = default!;
    }
}
