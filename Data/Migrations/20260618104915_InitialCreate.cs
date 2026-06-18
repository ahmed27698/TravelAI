using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameEs = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameFr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameZh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameTr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Continent = table.Column<string>(type: "text", nullable: false),
                    AverageBudgetPerDay = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Airline = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AirlineCode = table.Column<string>(type: "text", nullable: false),
                    AirlineLogo = table.Column<string>(type: "text", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    OriginCode = table.Column<string>(type: "text", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    DestinationCode = table.Column<string>(type: "text", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Class = table.Column<string>(type: "text", nullable: false),
                    IsDirectFlight = table.Column<bool>(type: "boolean", nullable: false),
                    SeatsAvailable = table.Column<int>(type: "integer", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "text", nullable: false),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DurationHours = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Stars = table.Column<double>(type: "double precision", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    HasPool = table.Column<bool>(type: "boolean", nullable: false),
                    HasWifi = table.Column<bool>(type: "boolean", nullable: false),
                    HasSpa = table.Column<bool>(type: "boolean", nullable: false),
                    IsLuxury = table.Column<bool>(type: "boolean", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Plan = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "AverageBudgetPerDay", "Code", "Continent", "Description", "ImageUrl", "IsPopular", "Name", "NameAr", "NameEs", "NameFr", "NameRu", "NameTr", "NameZh", "Rating" },
                values: new object[,]
                {
                    { 1, 80m, "EG", "Africa", "Land of the Pharaohs", "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800", true, "Egypt", "مصر", "Egipto", "Égypte", "Египет", "Mısır", "埃及", 5 },
                    { 2, 150m, "JP", "Asia", "Land of the Rising Sun", "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800", true, "Japan", "اليابان", "Japón", "Japon", "Япония", "Japonya", "日本", 5 },
                    { 3, 180m, "FR", "Europe", "City of Light and Art", "https://images.unsplash.com/photo-1499856871958-5b9357976b82?w=800", true, "France", "فرنسا", "Francia", "France", "Франция", "Fransa", "法国", 5 },
                    { 4, 70m, "TH", "Asia", "Land of Smiles", "https://images.unsplash.com/photo-1552465011-b4e21bf6e79a?w=800", true, "Thailand", "تايلاند", "Tailandia", "Thaïlande", "Таиланд", "Tayland", "泰国", 5 },
                    { 5, 160m, "IT", "Europe", "Eternal City & Cuisine", "https://images.unsplash.com/photo-1523906834658-6e24ef2386f9?w=800", true, "Italy", "إيطاليا", "Italia", "Italie", "Италия", "İtalya", "意大利", 5 },
                    { 6, 200m, "US", "Americas", "Land of Opportunities", "https://images.unsplash.com/photo-1485738422979-f5c462d49f74?w=800", true, "USA", "الولايات المتحدة", "EE.UU.", "États-Unis", "США", "ABD", "美国", 5 },
                    { 7, 350m, "MV", "Asia", "Tropical Paradise", "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=800", true, "Maldives", "جزر المالديف", "Maldivas", "Maldives", "Мальдивы", "Maldivler", "马尔代夫", 5 },
                    { 8, 90m, "TR", "Europe", "Where East Meets West", "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=800", true, "Turkey", "تركيا", "Turquía", "Turquie", "Турция", "Türkiye", "土耳其", 5 }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Id", "Airline", "AirlineCode", "AirlineLogo", "ArrivalTime", "Class", "DepartureTime", "Destination", "DestinationCode", "DurationMinutes", "IsDirectFlight", "IsFeatured", "Origin", "OriginCode", "Price", "SeatsAvailable" },
                values: new object[,]
                {
                    { 1, "Emirates", "EK", "", new DateTime(2026, 7, 15, 10, 30, 0, 0, DateTimeKind.Utc), "Economy", new DateTime(2026, 7, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Cairo", "CAI", 150, true, true, "Dubai", "DXB", 320m, 42 },
                    { 2, "Qatar Airways", "QR", "", new DateTime(2026, 7, 21, 7, 0, 0, 0, DateTimeKind.Utc), "Business", new DateTime(2026, 7, 20, 14, 0, 0, 0, DateTimeKind.Utc), "Tokyo", "NRT", 720, false, true, "Doha", "DOH", 890m, 8 },
                    { 3, "Air France", "AF", "", new DateTime(2026, 7, 18, 13, 0, 0, 0, DateTimeKind.Utc), "Economy", new DateTime(2026, 7, 18, 10, 30, 0, 0, DateTimeKind.Utc), "New York", "JFK", 510, true, true, "Paris", "CDG", 650m, 27 },
                    { 4, "Turkish Airlines", "TK", "", new DateTime(2026, 7, 22, 16, 0, 0, 0, DateTimeKind.Utc), "Economy", new DateTime(2026, 7, 22, 1, 0, 0, 0, DateTimeKind.Utc), "Bangkok", "BKK", 600, false, true, "Istanbul", "IST", 540m, 33 }
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "Category", "City", "CountryId", "Description", "DurationHours", "ImageUrl", "IsFeatured", "IsPopular", "Name", "Price", "Rating", "ReviewCount" },
                values: new object[,]
                {
                    { 1, "Adventure", "Cairo", 1, "Explore the ancient wonders with expert guide", 6, "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800", true, true, "Pyramids of Giza Tour", 85m, 4.9000000000000004, 5432 },
                    { 2, "Adventure", "Fujiyoshida", 2, "Guided trek to Japan's iconic peak", 10, "https://images.unsplash.com/photo-1490806843957-31f4c9a91c65?w=800", true, true, "Mount Fuji Hiking", 120m, 4.7999999999999998, 3211 },
                    { 3, "Museums", "Paris", 3, "Paris highlights with skip-the-line access", 8, "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=800", true, true, "Eiffel Tower & Louvre", 95m, 4.9000000000000004, 8765 },
                    { 4, "Food", "Bangkok", 4, "Learn authentic Thai cuisine from a local chef", 4, "https://images.unsplash.com/photo-1559314809-0d155014e29e?w=800", true, true, "Thai Cooking Masterclass", 55m, 4.9000000000000004, 2134 },
                    { 5, "Cultural", "Venice", 5, "Classic gondola ride through Venice canals", 3, "https://images.unsplash.com/photo-1534113414509-0eec2bfb493f?w=800", true, true, "Venice Gondola & Rialto", 75m, 4.7000000000000002, 4321 },
                    { 6, "Adventure", "Male", 7, "Discover vibrant coral reefs and marine life", 5, "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=800", true, true, "Maldives Snorkeling Safari", 180m, 5.0, 1876 }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "City", "CountryId", "Description", "HasPool", "HasSpa", "HasWifi", "ImageUrl", "IsFeatured", "IsLuxury", "Name", "PricePerNight", "Rating", "ReviewCount", "Stars" },
                values: new object[,]
                {
                    { 1, "", "Cairo", 1, "Iconic 5-star hotel overlooking the Nile River", true, true, true, "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800", true, true, "Nile Ritz-Carlton Cairo", 320m, 4.7999999999999998, 2341, 5.0 },
                    { 2, "", "Tokyo", 2, "Luxury high-rise with panoramic city views", true, true, true, "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800", true, true, "Park Hyatt Tokyo", 580m, 4.9000000000000004, 1876, 5.0 },
                    { 3, "", "Paris", 3, "Palace hotel in the heart of Paris since 1835", false, true, true, "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800", true, true, "Hôtel Le Meurice Paris", 750m, 4.9000000000000004, 3201, 5.0 },
                    { 4, "", "Bangkok", 4, "Serene riverside retreat with Thai architecture", true, true, true, "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800", true, true, "Anantara Riverside Bangkok", 210m, 4.7000000000000002, 1654, 5.0 },
                    { 5, "", "Florence", 5, "Renaissance palace turned luxury hotel", true, true, true, "https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=800", true, true, "Four Seasons Florence", 680m, 4.9000000000000004, 2109, 5.0 },
                    { 6, "", "Noonu Atoll", 7, "Overwater villas in pristine turquoise lagoon", true, true, true, "https://images.unsplash.com/photo-1439066290691-daf3a565efed?w=800", true, true, "Soneva Jani Maldives", 1800m, 5.0, 987, 5.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CountryId",
                table: "Activities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CountryId",
                table: "Hotels",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
