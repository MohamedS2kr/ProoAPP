using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Proo.APIs.Errors;
using Proo.APIs.Helpers;
using Proo.APIs.Hubs;
using Proo.APIs.Middlewares;
using Proo.Core.Contract;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Contract.Passenger_Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Core.Entities.Driver_Location;
using Proo.Infrastructer.Data;
using Proo.Infrastructer.Data.Context;
using Proo.Infrastructer.Identity.DataSeed;
using Proo.Infrastructer.Repositories;
using Proo.Infrastructer.Repositories.DriverRepository;
using Proo.Infrastructer.Repositories.Passenger_Repository;
using Proo.Infrastructer.Repositories.Ride_Repository;
using Proo.Service._RideService;
using Proo.Service.Identity;
using Proo.Service.LocationService;
using Proo.Service.Nearby_Driver_Service;
using Proo.Service.VehicleModelService;
using Proo.Service.VehicleTypeService;
using StackExchange.Redis;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;



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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme
                    , securityScheme: new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter the Bearer Authorization : `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header ,
                        Type = SecuritySchemeType.ApiKey ,
                        Scheme = "Bearer"
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    }, new string[]{}
                    }
                });
            });

            builder.Services.AddSignalR();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("DefualtConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(Connection);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                   );
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
            builder.Services.AddScoped(typeof(IRideRequestRepository), typeof(RideRequestRepository));
            builder.Services.AddScoped(typeof(IPassengerRepository), typeof(PassengerRepository));
            builder.Services.AddScoped(typeof(IRideRepository), typeof(RideRepository));
            builder.Services.AddScoped(typeof(IRideService), typeof(RideService));
            builder.Services.AddScoped<IVehicleTypeService, VehicleTypeService>();
            builder.Services.AddScoped<IVehicleModelService, VehicleModelService>();
            builder.Services.AddSingleton(typeof(IUpdateDriverLocationService), typeof(UpdateDriverLocationService));
            builder.Services.AddScoped(typeof(INearbyDriverService), typeof(NearbyDriversService));
            builder.Services.AddScoped(typeof(IRideAcceptanceService), typeof(RideAcceptanceService));


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
               // await _context.Database.MigrateAsync();
                await ApplicationIdentityDataSeed.SeedRoleForUserAsync(_roleManager);

            }
            catch (Exception ex)
            {
                var logger = loggerfactory.CreateLogger<Program>();
                logger.LogError(ex, "The Error Will logged Occured Apply Database");

            }

            #region Configure Kistrel Middleware
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("AllowAll");
            //app.UseRouting();
            app.UseMiddleware<ExeptionMiddleware>();
            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapHub<ChatHub>("/ChatHub");
            app.MapHub<RideHub>("/RideRequestHub");
            app.MapHub<RideRequestHub>("/notifyNearbyDrivers");
            app.MapHub<LocationHub>("/locationhub");
            app.MapControllers();
            
            #endregion


            #region WebSocket
            //app.UseWebSockets();


            //// ????? Endpoint ?? WebSocket
            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path == "/ws")
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            await Echo(context, webSocket); // ?? ?????? ???????
            //        }
            //        else
            //        {
            //            context.Response.StatusCode = 400; // Bad Request
            //        }
            //    }
            //    else
            //    {
            //        await next(); // ????? ?? ??????? ??????
            //    }
            //}); 
            #endregion

            app.Run();
        }

        #region WebSocket
        //private async Task Echo(HttpContext context, WebSocket webSocket)
        //{
        //    var buffer = new byte[1024 * 4];
        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        //    while (!result.CloseStatus.HasValue)
        //    {
        //        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //        Console.WriteLine($"Received: {receivedMessage}");

        //        // ????? ??????? ??? ???? ?????? ????????
        //        var location = JsonSerializer.Deserialize<DriverLocations>(receivedMessage);

        //        // ????? ?????? ?? Redis ?? ????? ????????
        //        await _redis.GeoAddAsync("drivers:locations", location.Longitude, location.Latitude, "driverId");

        //        // ?? ??? ??????
        //        await webSocket.SendAsync(
        //            new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Location Updated: {location.Latitude}, {location.Longitude}")),
        //            result.MessageType,
        //            result.EndOfMessage,
        //            CancellationToken.None);

        //        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    }

        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //} 
        #endregion

    }
}

