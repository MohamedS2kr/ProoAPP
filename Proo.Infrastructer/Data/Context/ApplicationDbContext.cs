using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proo.Core.Entities;
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
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<LocationHistory> LocationHistories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

    }
}
