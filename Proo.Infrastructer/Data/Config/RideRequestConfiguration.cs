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
    public class RideRequestConfiguration : IEntityTypeConfiguration<RideRequests>
    {
        public void Configure(EntityTypeBuilder<RideRequests> builder)
        {
            builder.Property(rr => rr.EstimatedDistance).HasColumnType("decimal(18,2)");
            builder.Property(rr => rr.EstimatedPrice).HasColumnType("decimal(18,2)");

            builder.HasOne(rr=>rr.Passenger).WithMany(p => p.RideRequests).HasForeignKey(p => p.PassengerId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(r => r.Status)
               .HasConversion(status => status.ToString(), StatusComparer => (Status)Enum.Parse(typeof(Status), StatusComparer));

        }
    }
}
