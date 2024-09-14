using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proo.APIs.Errors;
using Proo.APIs.Helpers;
using Proo.APIs.Middlewares;
using Proo.Core.Contract;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using Proo.Infrastructer.Data;
using Proo.Infrastructer.Data.Context;
using Proo.Infrastructer.Identity.DataSeed;
using Proo.Infrastructer.Repositories;
using Proo.Infrastructer.Repositories.DriverRepository;
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
       
            builder.Services.AddIdentity < ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
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

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });


            builder.Services.AddScoped<ITokenService, TokenServices>();
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfwork));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped(typeof(IGenaricRepositoy<>), typeof(GenaricRepository<>));
            builder.Services.AddScoped(typeof(IDriverRepository), typeof(DriverRepository));

            #endregion

            var app = builder.Build();
            // update database 
            using var scope = app.Services.CreateScope();
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

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
