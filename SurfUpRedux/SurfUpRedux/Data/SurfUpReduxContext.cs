using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurfUpRedux.Models;

namespace SurfUpRedux.Data
{
    public class SurfUpReduxContext : IdentityDbContext<SurfUpUser>
    {
        public SurfUpReduxContext (DbContextOptions<SurfUpReduxContext> options)
            : base(options)
        {
        }

        public DbSet<Board> Board { get; set; } = default!;
    }
}
