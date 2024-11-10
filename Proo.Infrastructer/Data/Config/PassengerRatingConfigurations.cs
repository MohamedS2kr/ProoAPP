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
    public class PassengerRatingConfigurations : IEntityTypeConfiguration<PassengerRating>
    {
        public void Configure(EntityTypeBuilder<PassengerRating> builder)
        {
            builder
                .HasOne(p => p.Passenger)
                .WithMany(pp => pp.PassengerRatings)
                .HasForeignKey(p => p.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.Ride)
             .WithMany()
             .HasForeignKey(pr => pr.RideId)
             .OnDelete(DeleteBehavior.NoAction); 

            builder.Property(pr => pr.Score).IsRequired();
            builder.Property(pr => pr.Review).HasColumnType("nvarchar(max)");
        }
    }
}
