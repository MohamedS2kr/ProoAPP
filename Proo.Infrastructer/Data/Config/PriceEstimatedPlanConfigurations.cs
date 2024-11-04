using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proo.Core.Entities.Price_Estimate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Data.Config
{
    internal class PriceEstimatedPlanConfigurations : IEntityTypeConfiguration<PriceEstimatedPlan>
    {
        public void Configure(EntityTypeBuilder<PriceEstimatedPlan> builder)
        {
            builder.Property(p => p.basePrice).HasColumnType("decimal");
            builder.Property(p => p.shortDistanceLimit).HasColumnType("decimal");
            builder.Property(p => p.shortDistancePrice).HasColumnType("decimal");
        }
    }
}
