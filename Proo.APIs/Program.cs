using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proo.APIs.Errors;
using Proo.APIs.Middlewares;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using Proo.Infrastructer.Identity.DataSeed;
using Proo.Service.Identity;
using StackExchange.Redis;
using System.Text;

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

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(Connection);
            });
            //builder.Services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = builder.Configuration.GetSection("Redis")["Configuration"];
            //});
            #region Identity
            builder.Services.AddIdentity < ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped<ITokenService,TokenServices>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters()
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                                    ValidateAudience = true,
                                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                                };
                            });
            #endregion
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
            var _roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            try
            {
                await _context.Database.MigrateAsync();
                await ApplicationIdentityDataSeed.SeedRoleForUserAsync(_roleManager);

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
            app.UseMiddleware<ExeptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            

            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
