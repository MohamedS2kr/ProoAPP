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
    public class VehicleModelConfigurations : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            builder.HasKey(vm => vm.Id);

            builder.Property(vm => vm.ModelName)
                   .IsRequired()
                   .HasMaxLength(100);

            // Define one-to-many relationship with VehicleType
            builder.HasOne(vm => vm.VehicleType)
                   .WithMany(vt => vt.vehicleModels)
                   .HasForeignKey(vm => vm.VehicleTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Define one-to-one relationship with Vehicle
            builder.HasOne(vm => vm.Vehicle)
                   .WithOne(v => v.vehicleModel)
                   .HasForeignKey<Vehicle>(v => v.VehicleModelId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
