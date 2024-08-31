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
    internal class PaymentConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Method)
                .HasConversion(method => method.ToString(), methodComperar => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), methodComperar));

            builder.Property(p => p.Status)
                .HasConversion(status => status.ToString(), StatusComp => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), StatusComp));

            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        }
    }
}
