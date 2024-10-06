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
    public class PassengerConfigurations : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasMany(p => p.Rides).WithOne(pp => pp.Passenger).HasForeignKey(p => p.PassengerId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
