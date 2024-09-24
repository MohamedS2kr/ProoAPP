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
            builder.OwnsOne(ride => ride.PickupLocation, pickuplocation => pickuplocation.WithOwner());

            builder.OwnsOne(ride => ride.DestinationLocation, destinationlocation => destinationlocation.WithOwner());

            builder.HasOne(r => r.Passenger).WithMany(p => p.Rides)
                .HasForeignKey(r => r.PassengerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Driver).WithMany(d => d.Rides)
                 .HasForeignKey(r => r.DriverId)
                 .OnDelete(DeleteBehavior.NoAction);

            //builder.HasOne(r => r.RideRequests).WithOne(rr => rr.Ride)
            //    .HasForeignKey<Ride>(r => r.RideRequestsId)
            //    .OnDelete(DeleteBehavior.NoAction);
            
            builder.Property(r => r.Status)
                .HasConversion(status => status.ToString(), StatusComparer => (RideStatus) Enum.Parse(typeof(RideStatus), StatusComparer));

            builder.Property(r => r.FarePrice).HasColumnType("decimal(18,2)");

        }
    }
}
