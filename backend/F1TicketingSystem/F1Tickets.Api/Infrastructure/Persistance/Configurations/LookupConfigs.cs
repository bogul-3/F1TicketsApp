using F1Tickets.Api.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Tickets.Api.Infrastructure.Persistance.Configurations
{
    public class LookupConfigs :
        IEntityTypeConfiguration<Country>,
        IEntityTypeConfiguration<City>,
        IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(x => x.Id);
        }

        void IEntityTypeConfiguration<City>.Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId);
        }

        void IEntityTypeConfiguration<PromoCode>.Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
