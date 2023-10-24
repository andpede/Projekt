using Microsoft.EntityFrameworkCore;
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


            //builder.Services.AddApiVersioning(options =>
            //{
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            //    options.ReportApiVersions = true;
            //    options.ApiVersionReader = ApiVersionReader.Combine(
            //        new QueryStringApiVersionReader("api-version"),
            //        new HeaderApiVersionReader("X-Version"));
            //    //new MediaTypeApiVersionReader("ver"));
            //});

            //builder.Services.AddVersionedApiExplorer(
            //    options =>
            //    {
            //        options.GroupNameFormat = "'v'VVV";
            //        options.SubstituteApiVersionInUrl = true;
            //    });


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