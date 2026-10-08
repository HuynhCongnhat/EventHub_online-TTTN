using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.BookingService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalQuantity = table.Column<int>(type: "integer", nullable: false),
                    SoldQuantity = table.Column<int>(type: "integer", nullable: false),
                    HeldQuantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketInventoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingItems_BookingOrders_BookingOrderId",
                        column: x => x.BookingOrderId,
                        principalTable: "BookingOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItems_TicketInventories_TicketInventoryId",
                        column: x => x.TicketInventoryId,
                        principalTable: "TicketInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingSeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketInventoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BookingOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    HeldUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingSeats_BookingOrders_BookingOrderId",
                        column: x => x.BookingOrderId,
                        principalTable: "BookingOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BookingSeats_TicketInventories_TicketInventoryId",
                        column: x => x.TicketInventoryId,
                        principalTable: "TicketInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingOrderId",
                table: "BookingItems",
                column: "BookingOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_TicketInventoryId",
                table: "BookingItems",
                column: "TicketInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingOrders_OrderCode",
                table: "BookingOrders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_BookingOrderId",
                table: "BookingSeats",
                column: "BookingOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_EventId_SeatCode",
                table: "BookingSeats",
                columns: new[] { "EventId", "SeatCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_TicketInventoryId",
                table: "BookingSeats",
                column: "TicketInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInventories_EventId_Name",
                table: "TicketInventories",
                columns: new[] { "EventId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingItems");

            migrationBuilder.DropTable(
                name: "BookingSeats");

            migrationBuilder.DropTable(
                name: "BookingOrders");

            migrationBuilder.DropTable(
                name: "TicketInventories");
        }
    }
}
