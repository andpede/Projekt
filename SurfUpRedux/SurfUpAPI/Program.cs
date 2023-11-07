using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using SurfUpReduxAPI.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;




namespace SurfUpAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SurfUpReduxContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("SurfUpReduxContext") ??
              throw new InvalidOperationException("Connection string 'SurfUpReduxContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            HttpClient httpClient = new();
            builder.Services.AddSingleton(typeof(HttpClient), httpClient);
            builder.Services.AddApiVersioning(Options =>
            {
                Options.AssumeDefaultVersionWhenUnspecified = true;
                Options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                Options.ReportApiVersions = true;
                Options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("x-version"));
            });


           

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseCors(cors => cors
            //.AllowAnyMethod()
            //.AllowAnyHeader()
            //.SetIsOriginAllowed(origin => true)
            //.AllowCredentials()
            //);

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}