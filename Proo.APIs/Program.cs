using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Errors;
using Proo.Infrastructer.Data.Context;

namespace Proo.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region services to the container.
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection"));
            });

            // generation response For validation errors [Factory]
            builder.Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    var errors = ActionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                         .SelectMany(p => p.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();
                    var response = new ApiValidationResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });


            #endregion

            var app = builder.Build();
            // update database 
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var _context = service.GetRequiredService<ApplicationDbContext>();
            var loggerfactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                await _context.Database.MigrateAsync();

            }
            catch (Exception ex)
            {
                var logger = loggerfactory.CreateLogger<Program>();
                logger.LogError(ex, "The Error Will logged Occured Apply Database");

            }

            #region Configure Kistrel Middleware
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
