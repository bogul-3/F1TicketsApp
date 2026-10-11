using F1Tickets.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Tickets.Api.Infrastructure.Persistance.Configurations
{
    public class ZoneDayConfig : IEntityTypeConfiguration<ZoneDay>
    {
        public void Configure(EntityTypeBuilder<ZoneDay> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SeatsLeft)
                .IsRequired();

            // 18 cifara, 2 posle zareza
            builder.Property(x => x.PriceAddon)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Capacity)
                .IsRequired();

            // Sprecava duple Zone - RaceDay kombinacije
            builder.HasIndex(x => new { x.ZoneId, x.RaceDayId })
                .IsUnique();
        }
    }
}
