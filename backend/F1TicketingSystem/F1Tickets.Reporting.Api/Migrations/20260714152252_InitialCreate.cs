using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Tickets.Reporting.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reporting_tickets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    zone_day_id = table.Column<Guid>(type: "uuid", nullable: false),
                    race_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    race_day_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchased_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_reporting_tickets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_reporting_tickets_purchased_on",
                table: "reporting_tickets",
                column: "purchased_on");

            migrationBuilder.CreateIndex(
                name: "i_x_reporting_tickets_race_day_id",
                table: "reporting_tickets",
                column: "race_day_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reporting_tickets");
        }
    }
}
