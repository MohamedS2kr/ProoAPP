using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proo.APIs.Errors;
using Proo.APIs.Helpers;
using Proo.APIs.Hubs;
using Proo.APIs.Middlewares;
using Proo.Core.Contract;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data;
using Proo.Infrastructer.Data.Context;
using Proo.Infrastructer.Identity.DataSeed;
using Proo.Infrastructer.Repositories;
using Proo.Infrastructer.Repositories.DriverRepository;
using Proo.Service._RideService;
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
            
            builder.Services.AddSignalR();



            // Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                };
            });

            // Add Swagger service
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Proo API",
                    Version = "v1",
                    Description = "API documentation for Proo application"
                });

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter JWT token with Bearer prefix: Bearer {token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection"),
         sqlOptions =>
         {
             sqlOptions.EnableRetryOnFailure(
                 maxRetryCount: 5,
                 maxRetryDelay: TimeSpan.FromSeconds(30),
                 errorNumbersToAdd: null);
             sqlOptions.CommandTimeout(180); // Set the command timeout here
         });

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
            builder.Services.AddScoped(typeof(IRideService), typeof(RideService));

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
            // if (app.Environment.IsDevelopment())
            // {
                app.UseSwagger();
                app.UseSwaggerUI();
            // }

           

            app.UseMiddleware<ExeptionMiddleware>();
            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proo API v1"));
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapHub<RideHub>("/rideHub");
            //});
            app.UseStaticFiles();

            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
