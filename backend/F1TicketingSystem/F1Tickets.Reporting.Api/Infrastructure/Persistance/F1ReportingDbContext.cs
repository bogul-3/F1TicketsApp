using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Reporting.Api.Infrastructure.Persistance
{
    public class F1ReportingDbContext : DbContext
    {
        public F1ReportingDbContext(DbContextOptions<F1ReportingDbContext> options) : base(options) { }

        public DbSet<ReportingTicket> ReportingTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReportingTicket>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.RaceDayId);
                entity.HasIndex(x => x.PurchasedOn);
            });

            // konverzija u snake_case
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // table
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                // columns
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }

                // keys
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(ToSnakeCase(key.GetName()));
                }

                // foreign keys
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.PrincipalKey.SetName(ToSnakeCase(fk.PrincipalKey.GetName()));
                }

                // indexes
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
                }
            }
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    if (i > 0) builder.Append("_");
                    builder.Append(char.ToLower(input[i]));
                }
                else
                {
                    builder.Append(input[i]);
                }
            }
            return builder.ToString();
        }
    }
}