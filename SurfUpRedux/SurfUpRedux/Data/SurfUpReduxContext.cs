using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurfUpRedux.Models;

namespace SurfUpRedux.Data
{
    public class SurfUpReduxContext : DbContext
    {
        public SurfUpReduxContext (DbContextOptions<SurfUpReduxContext> options)
            : base(options)
        {
        }

        public DbSet<Board> Board { get; set; } = default!;
    }
}
