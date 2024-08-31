using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Data.Config
{
    internal class RideConfigurations : IEntityTypeConfiguration<Ride>
    {
        public void Configure(EntityTypeBuilder<Ride> builder)
        {
            builder.HasOne(r => r.Passenger).WithMany(p => p.Rides)
                .HasForeignKey(r => r.PassengerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Driver).WithMany(d => d.Rides)
                 .HasForeignKey(r => r.DriverId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.Property(r => r.DropoffLocation).IsRequired();
            builder.Property(r => r.PickupLocation).IsRequired();

            builder.Property(r => r.Status)
                .HasConversion(status => status.ToString(), StatusComparer => (RideStatus) Enum.Parse(typeof(RideStatus), StatusComparer));

            builder.Property(r => r.Fare).HasColumnType("decimal(18,2)");

        }
    }
}
