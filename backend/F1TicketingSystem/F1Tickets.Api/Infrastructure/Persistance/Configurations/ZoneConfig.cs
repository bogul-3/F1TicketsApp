using F1Tickets.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Tickets.Api.Infrastructure.Persistance.Configurations
{
    public class ZoneConfig : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(x => x.ZoneDays)
                .WithOne(x => x.Zone)
                .HasForeignKey(x => x.ZoneId);
        }
    }
}
