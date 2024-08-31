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
    internal class VehicleConfigurations : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.Property(v => v.Model).IsRequired().HasMaxLength(100);
            builder.Property(v => v.Color).IsRequired().HasMaxLength(100);
            builder.Property(v => v.Make).IsRequired().HasMaxLength(100);
            builder.Property(v => v.LicensePlate).IsRequired().HasMaxLength(100);

            
        }
    }
}
