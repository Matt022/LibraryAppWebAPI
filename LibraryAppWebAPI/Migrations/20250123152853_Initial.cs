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
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanManipulate = table.Column<bool>(type: "bit", nullable: false)
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
                    Author = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvailableCopies = table.Column<int>(type: "int", nullable: false),
                    TotalAvailableCopies = table.Column<int>(type: "int", nullable: false),
                    CanManipulate = table.Column<bool>(type: "bit", nullable: false),
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
                columns: new[] { "Id", "CanManipulate", "DateOfBirth", "FirstName", "LastName", "PersonalId" },
                values: new object[,]
                {
                    { 1, false, new DateTime(1990, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", "Smith", "123456789" },
                    { 2, false, new DateTime(1985, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emily", "Johnson", "987654321" },
                    { 3, false, new DateTime(1978, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michael", "Williams", "456123789" },
                    { 4, false, new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sarah", "Brown", "852963741" },
                    { 5, false, new DateTime(1982, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "Jones", "159753486" },
                    { 6, false, new DateTime(2000, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emma", "Garcia", "753159846" },
                    { 7, false, new DateTime(1998, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "James", "Martinez", "951357486" },
                    { 8, false, new DateTime(1993, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sophia", "Hernandez", "789654123" },
                    { 9, false, new DateTime(1975, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Christopher", "Lopez", "321654987" },
                    { 10, false, new DateTime(2001, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Olivia", "Gonzalez", "147258369" },
                    { 11, false, new DateTime(1988, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Daniel", "Perez", "963852741" },
                    { 12, false, new DateTime(1992, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ava", "Wilson", "456789123" },
                    { 13, false, new DateTime(1980, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matthew", "Anderson", "258147369" },
                    { 14, false, new DateTime(1999, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Isabella", "Thomas", "741852963" },
                    { 15, false, new DateTime(1983, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ethan", "Taylor", "369147258" },
                    { 16, false, new DateTime(2002, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mia", "Moore", "654987321" },
                    { 17, false, new DateTime(1977, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "White", "987321654" },
                    { 18, false, new DateTime(1994, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Charlotte", "Harris", "123789456" },
                    { 19, false, new DateTime(1989, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Liam", "Clark", "951753852" },
                    { 20, false, new DateTime(2003, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amelia", "Lewis", "789321654" }
                });

            migrationBuilder.InsertData(
                table: "Title",
                columns: new[] { "Id", "Author", "AvailableCopies", "CanManipulate", "Discriminator", "ISBN", "Name", "NumberOfPages", "TotalAvailableCopies" },
                values: new object[,]
                {
                    { 1, "J.K. Rowling", 12, false, "Book", "9780747532699", "Harry Potter and the Philosopher's Stone", 223, 12 },
                    { 2, "George Orwell", 8, false, "Book", "9780451524935", "1984", 328, 8 },
                    { 3, "J.R.R. Tolkien", 5, false, "Book", "9780544003415", "The Lord of the Rings", 1178, 5 },
                    { 4, "F. Scott Fitzgerald", 7, false, "Book", "9780743273565", "The Great Gatsby", 180, 7 },
                    { 5, "Harper Lee", 10, false, "Book", "9780061120084", "To Kill a Mockingbird", 281, 10 },
                    { 6, "Jane Austen", 6, false, "Book", "9781503290563", "Pride and Prejudice", 279, 6 },
                    { 7, "Mark Twain", 9, false, "Book", "9780486280615", "Adventures of Huckleberry Finn", 366, 9 },
                    { 8, "Mary Shelley", 4, false, "Book", "9780486282114", "Frankenstein", 280, 4 },
                    { 9, "Charlotte Brontë", 5, false, "Book", "9780141441146", "Jane Eyre", 500, 5 },
                    { 10, "Herman Melville", 3, false, "Book", "9781503280786", "Moby-Dick", 635, 3 },
                    { 11, "Leo Tolstoy", 2, false, "Book", "9780198800545", "War and Peace", 1225, 2 },
                    { 12, "Gabriel García Márquez", 6, false, "Book", "9780060883287", "One Hundred Years of Solitude", 417, 6 },
                    { 13, "Ernest Hemingway", 8, false, "Book", "9780684801223", "The Old Man and the Sea", 127, 8 },
                    { 14, "William Shakespeare", 10, false, "Book", "9780140714548", "Hamlet", 342, 10 },
                    { 15, "Oscar Wilde", 7, false, "Book", "9780141439570", "The Picture of Dorian Gray", 254, 7 },
                    { 16, "George R.R. Martin", 5, false, "Book", "9780553593716", "A Game of Thrones", 835, 5 },
                    { 17, "Aldous Huxley", 6, false, "Book", "9780060850524", "Brave New World", 288, 6 },
                    { 18, "Kurt Vonnegut", 9, false, "Book", "9780440180296", "Slaughterhouse-Five", 275, 9 },
                    { 19, "Emily Brontë", 4, false, "Book", "9780141439556", "Wuthering Heights", 416, 4 },
                    { 20, "J.D. Salinger", 8, false, "Book", "9780316769488", "The Catcher in the Rye", 214, 8 }
                });

            migrationBuilder.InsertData(
                table: "Title",
                columns: new[] { "Id", "Author", "AvailableCopies", "CanManipulate", "Discriminator", "Name", "NumberOfMinutes", "PublishYear", "TotalAvailableCopies" },
                values: new object[,]
                {
                    { 21, "Christopher Nolan", 7, false, "Dvd", "Inception", 148, 2010, 7 },
                    { 22, "Steven Spielberg", 5, false, "Dvd", "Jurassic Park", 127, 1993, 5 },
                    { 23, "Peter Jackson", 8, false, "Dvd", "The Lord of the Rings: The Fellowship of the Ring", 178, 2001, 8 },
                    { 24, "Quentin Tarantino", 6, false, "Dvd", "Pulp Fiction", 154, 1994, 6 },
                    { 25, "James Cameron", 10, false, "Dvd", "Titanic", 195, 1997, 10 },
                    { 26, "Ridley Scott", 4, false, "Dvd", "Gladiator", 155, 2000, 4 },
                    { 27, "Francis Ford Coppola", 6, false, "Dvd", "The Godfather", 175, 1972, 6 },
                    { 28, "Martin Scorsese", 5, false, "Dvd", "The Wolf of Wall Street", 180, 2013, 5 },
                    { 29, "Robert Zemeckis", 9, false, "Dvd", "Forrest Gump", 142, 1994, 9 },
                    { 30, "Stanley Kubrick", 10, false, "Dvd", "The Shining", 146, 1980, 10 },
                    { 31, "George Lucas", 8, false, "Dvd", "Star Wars: Episode IV - A New Hope", 121, 1977, 8 },
                    { 32, "Christopher Nolan", 6, false, "Dvd", "The Dark Knight", 152, 2008, 6 },
                    { 33, "David Fincher", 5, false, "Dvd", "Fight Club", 139, 1999, 5 },
                    { 34, "Alfonso Cuarón", 7, false, "Dvd", "Gravity", 91, 2013, 7 },
                    { 35, "Guillermo del Toro", 4, false, "Dvd", "Pan's Labyrinth", 118, 2006, 4 },
                    { 36, "Baz Luhrmann", 6, false, "Dvd", "The Great Gatsby", 143, 2013, 6 },
                    { 37, "Jon Favreau", 8, false, "Dvd", "Iron Man", 126, 2008, 8 },
                    { 38, "James Cameron", 5, false, "Dvd", "Avatar", 162, 2009, 5 },
                    { 39, "Frank Darabont", 10, false, "Dvd", "The Shawshank Redemption", 142, 1994, 10 },
                    { 40, "Damien Chazelle", 6, false, "Dvd", "La La Land", 128, 2016, 6 }
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
