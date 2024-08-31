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
    internal class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(U => U.FirstName).IsRequired().HasMaxLength(20);
            builder.Property(U => U.LastName).IsRequired().HasMaxLength(20);

            builder.Property(U => U.Gender)
                .HasConversion(gender => gender.ToString(), genderComparer => (Gender) Enum.Parse(typeof(Gender), genderComparer));
        }
    }
}
