using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OdeonFlightInvoiceCase.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingID = table.Column<int>(type: "integer", nullable: false),
                    Customer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CarrierCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    FlightNo = table.Column<int>(type: "integer", nullable: false),
                    FlightDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Origin = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Destination = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoiceNumber = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FlightDate_CarrierCode_FlightNo",
                table: "Reservations",
                columns: new[] { "FlightDate", "CarrierCode", "FlightNo" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_InvoiceNumber",
                table: "Reservations",
                column: "InvoiceNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
