using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiIsocare2.Migrations
{
    /// <inheritdoc />
    public partial class AddForgetPasswordToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "queue_status",
                columns: table => new
                {
                    queue_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    queue_status_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_queue_status", x => x.queue_status_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "queue_type",
                columns: table => new
                {
                    queue_type_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_queue_type", x => x.queue_type_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    firstname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    citizen_id_number = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reset_token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResetTokenExpiry = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "counter_queue",
                columns: table => new
                {
                    queue_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    queue_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    queue_status_id = table.Column<int>(type: "int", nullable: false),
                    queue_type_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    queue_number = table.Column<int>(type: "int", nullable: false),
                    counter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counter_queue", x => x.queue_id);
                    table.ForeignKey(
                        name: "FK_counter_queue_queue_status_queue_status_id",
                        column: x => x.queue_status_id,
                        principalTable: "queue_status",
                        principalColumn: "queue_status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_counter_queue_queue_type_queue_type_id",
                        column: x => x.queue_type_id,
                        principalTable: "queue_type",
                        principalColumn: "queue_type_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "booking_queue",
                columns: table => new
                {
                    queue_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    queue_type_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    queue_number = table.Column<int>(type: "int", nullable: false),
                    queue_status_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    booking_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    appointment_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    counter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_queue", x => x.queue_id);
                    table.ForeignKey(
                        name: "FK_booking_queue_queue_status_queue_status_id",
                        column: x => x.queue_status_id,
                        principalTable: "queue_status",
                        principalColumn: "queue_status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_queue_queue_type_queue_type_id",
                        column: x => x.queue_type_id,
                        principalTable: "queue_type",
                        principalColumn: "queue_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_queue_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_booking_queue_queue_status_id",
                table: "booking_queue",
                column: "queue_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_queue_queue_type_id",
                table: "booking_queue",
                column: "queue_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_queue_user_id",
                table: "booking_queue",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_counter_queue_queue_status_id",
                table: "counter_queue",
                column: "queue_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_counter_queue_queue_type_id",
                table: "counter_queue",
                column: "queue_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_queue");

            migrationBuilder.DropTable(
                name: "counter_queue");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "queue_status");

            migrationBuilder.DropTable(
                name: "queue_type");
        }
    }
}
