using F1Tickets.Api.Domain.Enums;
using F1Tickets.Reporting.Api.Infrastructure.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly F1ReportingDbContext _db;

        public ReportingController(F1ReportingDbContext db)
        {
            _db = db;
        }

        // DTOs
        public class TicketDto
        {
            public Guid TicketId { get; set; }
            public Guid userId { get; set; }
            public Guid ZoneDayId { get; set; }
            public Guid RaceDayId { get; set; }
            public Guid RaceEventId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
            public DateTime PurchasedOn { get; set; }
        }

        public class SalesReportDto
        {
            public Guid RaceDayId { get; set; }
            public int TicketsSold { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        public class SalesByDateDto
        {
            public DateTime Date { get; set; }
            public int TicketsSold { get; set; }
        }

       
        // ---------------------------------------
        // Get tickets by RaceDay
        [HttpGet("racedays/{raceDayId}/tickets")]
        public async Task<IActionResult> GetTicketsByRaceDay(Guid raceDayId)
        {
            var tickets = await _db.ReportingTickets
                .Where(t => t.RaceDayId == raceDayId && t.Status == TicketStatus.ACTIVE)
                .OrderBy(t => t.PurchasedOn)
                .Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    userId = t.userId,
                    ZoneDayId = t.ZoneDayId,
                    RaceDayId = t.RaceDayId,
                    RaceEventId = t.RaceEventId,
                    Quantity = t.Quantity,
                    TotalPrice = t.TotalPrice,
                    PurchasedOn = t.PurchasedOn
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // ---------------------------------------
        // Aggregated sales by purchase date
        // GET: /api/reporting/sales-by-date?from=2026-01-01&to=2026-12-31
        [HttpGet("sales-by-date")]
        public async Task<IActionResult> GetSalesByDate(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query = _db.ReportingTickets
                .Where(t => t.Status == TicketStatus.ACTIVE)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(t => t.PurchasedOn >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.PurchasedOn <= to.Value);

            var result = await query
                .GroupBy(t => t.PurchasedOn.Date)
                .Select(g => new SalesByDateDto
                {
                    Date = g.Key,
                    TicketsSold = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return Ok(result);
        }
    }
}