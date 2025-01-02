using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryAppWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersonalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Title",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvailableCopies = table.Column<int>(type: "int", nullable: false),
                    TotalAvailableCopies = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NumberOfPages = table.Column<int>(type: "int", nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PublishYear = table.Column<int>(type: "int", nullable: true),
                    NumberOfMinutes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Title", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MessageContext = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MessageSubject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SendData = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueueItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    TimeAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TitleId = table.Column<int>(type: "int", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueItems_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueItems_Title_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Title",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    RentedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TitleId = table.Column<int>(type: "int", nullable: false),
                    TimesProlongued = table.Column<int>(type: "int", nullable: false),
                    TitleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalEntries_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalEntries_Title_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Title",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "LastName", "PersonalId" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", "Doe", "123456789" },
                    { 2, new DateTime(1985, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane", "Smith", "987654321" }
                });

            migrationBuilder.InsertData(
                table: "Title",
                columns: new[] { "Id", "Author", "AvailableCopies", "Discriminator", "ISBN", "Name", "NumberOfPages", "TotalAvailableCopies" },
                values: new object[,]
                {
                    { 1, "George Orwell", 5, "Book", "978-0451524935", "1984", 328, 5 },
                    { 2, "Aldous Huxley", 3, "Book", "978-0060850524", "Brave New World", 268, 3 }
                });

            migrationBuilder.InsertData(
                table: "Title",
                columns: new[] { "Id", "Author", "AvailableCopies", "Discriminator", "Name", "NumberOfMinutes", "PublishYear", "TotalAvailableCopies" },
                values: new object[,]
                {
                    { 3, "Christopher Nolan", 4, "Dvd", "Inception", 148, 2010, 4 },
                    { 4, "Steven Spielberg", 2, "Dvd", "Jurassic Park", 127, 1993, 2 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "MemberId", "MessageContext", "MessageSubject", "SendData" },
                values: new object[,]
                {
                    { 1, 1, "Dear John Doe, we are delighted to welcome you to our library. Explore our collection and enjoy our services!", "Welcome to the Library!", new DateTime(2025, 1, 2, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7302) },
                    { 2, 2, "Dear Jane Smith, we are delighted to welcome you to our library. Explore our collection and enjoy our services!", "Welcome to the Library!", new DateTime(2025, 1, 2, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7306) }
                });

            migrationBuilder.InsertData(
                table: "RentalEntries",
                columns: new[] { "Id", "MaxReturnDate", "MemberId", "RentedDate", "ReturnDate", "TimesProlongued", "TitleId", "TitleType" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 16, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7264), 1, new DateTime(2024, 12, 26, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7257), null, 0, 1, 1 },
                    { 2, new DateTime(2025, 2, 2, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7270), 2, new DateTime(2024, 12, 23, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7270), null, 1, 2, 1 },
                    { 3, new DateTime(2025, 1, 8, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7272), 1, new DateTime(2025, 1, 2, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7271), new DateTime(2025, 1, 7, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7272), 0, 3, 2 },
                    { 4, new DateTime(2025, 1, 5, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7274), 2, new DateTime(2024, 12, 29, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7274), new DateTime(2025, 1, 3, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7275), 0, 4, 2 },
                    { 5, new DateTime(2024, 12, 13, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7276), 1, new DateTime(2024, 12, 3, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7276), new DateTime(2024, 12, 15, 16, 59, 55, 689, DateTimeKind.Utc).AddTicks(7277), 0, 1, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MemberId",
                table: "Messages",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_MemberId",
                table: "QueueItems",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_TitleId",
                table: "QueueItems",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalEntries_MemberId",
                table: "RentalEntries",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalEntries_TitleId",
                table: "RentalEntries",
                column: "TitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "QueueItems");

            migrationBuilder.DropTable(
                name: "RentalEntries");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Title");
        }
    }
}
