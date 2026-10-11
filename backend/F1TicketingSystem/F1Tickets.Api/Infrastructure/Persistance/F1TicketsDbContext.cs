using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace F1Tickets.Api.Infrastructure.Persistance
{
    public class F1TicketsDbContext : DbContext
    {
        public F1TicketsDbContext(DbContextOptions<F1TicketsDbContext> options) : base(options)
        { 
        }

        public DbSet<RaceEvent> RaceEvents => Set<RaceEvent>();
        public DbSet<RaceDay> RaceDays => Set<RaceDay>();
        public DbSet<Zone> Zones => Set<Zone>();
        public DbSet<ZoneDay> ZoneDays => Set<ZoneDay>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<User> Users => Set<User>();

        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(F1TicketsDbContext).Assembly);

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.UserId, t.ZoneDayId })
                .HasFilter("status = 1");

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(ToSnakeCase(property.Name));

                foreach (var key in entity.GetKeys())
                    key.SetName(ToSnakeCase(key.GetName()));

                foreach (var fk in entity.GetForeignKeys())
                    fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));

                foreach (var index in entity.GetIndexes())
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }
        }

        // Helper Function
        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var sb = new StringBuilder();
            var previousWasUpper = false;

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (char.IsUpper(c))
                {
                    if (i > 0 && !previousWasUpper)
                        sb.Append("_");

                    sb.Append(char.ToLowerInvariant(c));
                    previousWasUpper = true;
                }
                else
                {
                    sb.Append(c);
                    previousWasUpper = false;
                }
            }

            return sb.ToString();
        }
    }
}
