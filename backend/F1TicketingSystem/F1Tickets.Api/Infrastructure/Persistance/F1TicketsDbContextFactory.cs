using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace F1Tickets.Api.Infrastructure.Persistance
{
    public class F1TicketsDbContextFactory : IDesignTimeDbContextFactory<F1TicketsDbContext>
    {
        public F1TicketsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<F1TicketsDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=f1tickets;Username=postgres;Password=postgres");
            return new F1TicketsDbContext(optionsBuilder.Options);
        }
    }
}