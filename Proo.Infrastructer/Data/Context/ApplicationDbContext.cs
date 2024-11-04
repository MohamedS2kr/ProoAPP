using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proo.Core.Entities;
using Proo.Core.Entities.Price_Estimate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Passenger> Passengers { get; set; }

        public DbSet<Bid> Bids { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PassengerRating> PassengerRatings { get; set; }
        public DbSet<DriverRating> DriverRatings { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<RideRequests> RideRequests { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<CategoryOfVehicle> CategoryOfVehicles { get; set; }
        public DbSet<PriceEstimatedPlan> priceEstimatedPlans { get; set; }

        public DbSet<PricePerDistance> pricePerDistances { get; set; }
        public DbSet<PriceCategoryTier> priceCategoryTiers { get; set; }
    }
}
