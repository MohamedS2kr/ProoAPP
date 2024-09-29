using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Data.Config
{
    public class CategoryOfVehicleConfiguration : IEntityTypeConfiguration<CategoryOfVehicle>
    {
        public void Configure(EntityTypeBuilder<CategoryOfVehicle> builder)
        {
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.Name).IsRequired();
           
        }
    }
}
