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
    internal class DriverConfigurations : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.Property(d => d.LicenseNumber).IsRequired();
            builder.Property(d => d.VehicleDetails).IsRequired();
        }
    }
}
