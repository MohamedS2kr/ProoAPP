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
    internal class BidConfigurations : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.HasOne(b => b.Driver)
                .WithMany()
                .HasForeignKey(b => b.DriverId);

            builder.HasOne(b => b.Ride)
                .WithMany()
                .HasForeignKey(b => b.RideId);

            builder.Property(b => b.OfferedPrice).HasColumnType("decimal(10,2)");
        }
    }
}
