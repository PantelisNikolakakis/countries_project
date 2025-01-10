using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VivaProject.Entities;

namespace VivaProject.Configurations
{
    public class CountryConfigurations : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Country", "country");
            builder.Property(c => c.Name)
                .HasMaxLength(1000)
                .IsRequired();
            builder.Property(c => c.Capital)
                .HasMaxLength(1000)
                .IsRequired();
            builder.Property(c => c.Borders)
                .IsRequired(false);
        }
    }
}
