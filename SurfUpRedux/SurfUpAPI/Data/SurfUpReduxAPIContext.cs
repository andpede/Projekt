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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Booking>()
        //        .HasOne(bo => bo.Board)
        //        .WithOne(b => b.Booking)
        //        .HasForeignKey<Booking>(bo => bo.BoardId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<Booking>()
        //        .HasOne(bo => bo.User)
        //        .WithMany(u => u.Bookings)
        //        .HasForeignKey(bo => bo.UserId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}

    }
}
