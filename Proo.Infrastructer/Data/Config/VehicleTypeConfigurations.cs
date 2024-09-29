using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Data.Config
{
    public class VehicleTypeConfigurations : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder)
        {
            builder.HasKey(vt => vt.Id);

            builder.Property(vt => vt.TypeName)
                   .IsRequired()
                   .HasMaxLength(100);

            //  one-to-many relationship with VehicleModels
            builder.HasMany(vt => vt.vehicleModels)
                   .WithOne(vm => vm.VehicleType)
                   .HasForeignKey(vm => vm.VehicleTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // relationship with CategoryOfVehicle
            builder.HasOne(vt => vt.CategoryOfVehicle)
                   .WithMany(c => c.VehicleTypes)
                   .HasForeignKey(vt => vt.CategoryOfVehicleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}