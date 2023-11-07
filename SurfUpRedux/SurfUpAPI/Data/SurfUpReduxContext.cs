using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurfUpReduxAPI.Models;

namespace SurfUpReduxAPI.Data
{
    public class SurfUpReduxContext : IdentityDbContext
    {
        public SurfUpReduxContext(DbContextOptions<SurfUpReduxContext> options)
            : base(options)
        {
        }

        public DbSet<Board> Board { get; set; } = default!;
        public DbSet<Booking> Booking { get; set; } = default!;

      

    }
}
