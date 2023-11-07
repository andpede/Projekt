using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfUpRedux.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SurfUpRedux.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new SurfUpReduxContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfUpReduxContext>>());

            // Seed boards if not exists
            SeedBoards(context);

            // Seed roles and users
            var userManager = serviceProvider.GetRequiredService<UserManager<SurfUpUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private static void SeedBoards(SurfUpReduxContext context)
        {
            if (!context.Board.Any())
            {
                context.Board.AddRange(
                    new Board
                    {
                        Name = "The Minilog",
                        Length = 6,
                        Width = 21,
                        Thickness = 2.75M,
                        Volume = 38.3M,
                        Type = "Shortboard",
                        Price = 565,
                        Equipment = null,
                        ImageUrl = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/9455175_orig.jpg",
                    },

                    new Board
                    {
                        Name = "The Wide Glider",
                        Length = 7.1M,
                        Width = 21.75M,
                        Thickness = 2.75M,
                        Volume = 44.16M,
                        Type = "Funboard",
                        Price = 685,
                        Equipment = null,
                        ImageUrl = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/1768109_orig.jpg",
                    },

                    new Board
                    {
                        Name = "The Golden Ratio",
                        Length = 6.3M,
                        Width = 21.85M,
                        Thickness = 2.9M,
                        Volume = 43.22M,
                        Type = "Funboard",
                        Price = 695,
                        Equipment = null,
                        ImageUrl = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/7016725_orig.jpg",
                    },

                    new Board
                    {
                        Name = "Mahi Mahi",
                        Length = 5.4M,
                        Width = 20.75M,
                        Thickness = 2.3M,
                        Volume = 29.39M,
                        Type = "Fish",
                        Price = 645,
                        Equipment = null,
                        ImageUrl = "https://kite-prod.b-cdn.net/19205-home_default/slingshot-flying-fish-v2-2023-wing-foilboard.jpg",
                    },

                    new Board
                    {
                        Name = "The Emerald Glider",
                        Length = 9.2M,
                        Width = 22.8M,
                        Thickness = 2.8M,
                        Volume = 65.4M,
                        Type = "Longboard",
                        Price = 895,
                        Equipment = null,
                        ImageUrl = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/6384943_orig.jpg",
                    },

                    new Board
                    {
                        Name = "The Bomb",
                        Length = 5.5M,
                        Width = 21,
                        Thickness = 2.5M,
                        Volume = 33.7M,
                        Type = "Shortboard",
                        Price = 645,
                        Equipment = null,
                        ImageUrl = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/_3074463_orig.jpg",
                    },

                    new Board
                    {
                        Name = "Walden Magic",
                        Length = 9.6M,
                        Width = 19.4M,
                        Thickness = 3,
                        Volume = 80,
                        Type = "Longboard",
                        Price = 1025,
                        Equipment = null,
                        ImageUrl = "https://www.light-surfboards.com/uploads/5/7/3/0/57306051/_9897519_orig.jpg",
                    },

                    new Board
                    {
                        Name = "Naish One",
                        Length = 12.6M,
                        Width = 30,
                        Thickness = 6,
                        Volume = 301,
                        Type = "SUP",
                        Price = 854,
                        Equipment = "Paddle",
                        ImageUrl = "https://kite-prod.b-cdn.net/9006-thickbox_default/jp-carbon-pro-83-2021-ctl-paddle.jpg",
                    },

                    new Board
                    {
                        Name = "Six Tourer",
                        Length = 11.6M,
                        Width = 32,
                        Thickness = 6,
                        Volume = 270,
                        Type = "SUP",
                        Price = 611,
                        Equipment = "Fin, Paddle, Pump, Leash",
                        ImageUrl = "https://www.light-sup.com/uploads/5/7/3/0/57306051/s795737404132599590_p388_i15_w3002.jpeg?width=640",
                    },

                    new Board
                    {
                        Name = "Naish Maliko",
                        Length = 14,
                        Width = 25,
                        Thickness = 6,
                        Volume = 330,
                        Type = "SUP",
                        Price = 1304,
                        Equipment = "Fin, Paddle, Pump, Leash",
                        ImageUrl = "https://www.light-sup.com/uploads/5/7/3/0/57306051/s795737404132599590_p389_i13_w3024.jpeg?width=640",
                    }
                );
                context.SaveChanges();
            }       
        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Manager"))
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }
        }

        private static async Task SeedUsers(UserManager<SurfUpUser> userManager)
        {
            const string defaultPassword = "Password123!";

            // Seed normal users
            for (int i = 1; i <= 8; i++)
            {
                var user = new SurfUpUser { UserName = $"user{i}@surfup.com", Email = $"user{i}@surfup.com" };
                if (await userManager.FindByNameAsync(user.UserName) == null)
                {
                    await userManager.CreateAsync(user, defaultPassword);
                    await userManager.AddToRoleAsync(user, "User");
                }

            }

            // Seed admin
            var admin = new SurfUpUser { UserName = "admin@surfup.com", Email = "admin@surfup.com" };
            if (await userManager.FindByNameAsync(admin.UserName) == null)
            {
                await userManager.CreateAsync(admin, defaultPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed manager
            var manager = new SurfUpUser { UserName = "manager@surfup.com", Email = "manager@surfup.com" };
            if (await userManager.FindByNameAsync(manager.UserName) == null)
            {
                await userManager.CreateAsync(manager, defaultPassword);
                await userManager.AddToRoleAsync(manager, "Manager");
            }
        }
    }
}

