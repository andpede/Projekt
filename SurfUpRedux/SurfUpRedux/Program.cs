using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfUpRedux.Data;
using SurfUpRedux.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;

namespace SurfUpRedux
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<SurfUpReduxContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SurfUpReduxContext") ?? 
                throw new InvalidOperationException("Connection string 'SurfUpReduxContext' not found.")));

            builder.Services.AddDefaultIdentity<SurfUpUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<SurfUpReduxContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton(typeof(HttpClient), httpClient);


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await SeedData.Initialize(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.UseRequestLocalization("da-DK");

            app.Run();
        }
    }
}