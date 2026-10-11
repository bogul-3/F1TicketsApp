using F1Tickets.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Tickets.Api.Infrastructure.Persistance.Configurations
{
    public class RaceEventConfig : IEntityTypeConfiguration<RaceEvent>
    {
        public void Configure(EntityTypeBuilder<RaceEvent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.AdditionalInfo)
                .HasMaxLength(1000);

            builder.HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.RaceDays)
                .WithOne(x => x.RaceEvent)
                .HasForeignKey(x => x.RaceEventId);
        }
    }
}
