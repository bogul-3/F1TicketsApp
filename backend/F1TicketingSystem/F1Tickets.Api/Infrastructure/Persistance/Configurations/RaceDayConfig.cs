using F1Tickets.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Tickets.Api.Infrastructure.Persistance.Configurations
{
    public class RaceDayConfig : IEntityTypeConfiguration<RaceDay>
    {
        public void Configure(EntityTypeBuilder<RaceDay> builder)
        {
            builder.HasKey(x => x.Id);

            // 18 cifara, 2 posle zareza
            builder.Property(x => x.BasePrice)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(x => x.ZoneDays)
                .WithOne(x => x.RaceDay)
                .HasForeignKey(x => x.RaceDayId);
        }
    }
}
