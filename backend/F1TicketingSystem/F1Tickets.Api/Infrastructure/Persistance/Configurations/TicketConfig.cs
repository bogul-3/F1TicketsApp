using F1Tickets.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Tickets.Api.Infrastructure.Persistance.Configurations
{
    public class TicketConfig : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Status).HasConversion<int>();
            builder.Property(x => x.AccessCode).HasMaxLength(100);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.PromoCode)
                .WithMany()
                .HasForeignKey(x => x.PromoCodeId)
                .IsRequired(false);

            builder.HasOne(x => x.ZoneDay)
                .WithMany(z => z.Tickets)
                .HasForeignKey(x => x.ZoneDayId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
