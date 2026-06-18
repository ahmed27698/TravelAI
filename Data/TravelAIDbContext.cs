using Microsoft.EntityFrameworkCore;
using TravelAI.Models;

namespace TravelAI.Data;

public class TravelAIDbContext : DbContext
{
    public TravelAIDbContext(DbContextOptions<TravelAIDbContext> options) : base(options) { }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Country>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.Property(c => c.AverageBudgetPerDay).HasColumnType("numeric(10,2)");
        });

        modelBuilder.Entity<Hotel>(e =>
        {
            e.Property(h => h.PricePerNight).HasColumnType("numeric(10,2)");
            e.HasOne(h => h.Country).WithMany(c => c.Hotels).HasForeignKey(h => h.CountryId);
        });

        modelBuilder.Entity<Flight>(e =>
        {
            e.Property(f => f.Price).HasColumnType("numeric(10,2)");
        });

        modelBuilder.Entity<Activity>(e =>
        {
            e.Property(a => a.Price).HasColumnType("numeric(10,2)");
            e.HasOne(a => a.Country).WithMany(c => c.Activities).HasForeignKey(a => a.CountryId);
        });

        modelBuilder.Entity<Subscription>(e =>
        {
            e.Property(s => s.Price).HasColumnType("numeric(10,2)");
            e.HasOne(s => s.User).WithOne(u => u.Subscription).HasForeignKey<Subscription>(s => s.UserId);
        });

        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.HasOne(c => c.User).WithMany(u => u.ChatMessages).HasForeignKey(c => c.UserId);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "Egypt", NameAr = "مصر", NameEs = "Egipto", NameFr = "Égypte", NameZh = "埃及", NameRu = "Египет", NameTr = "Mısır", Code = "EG", Continent = "Africa", AverageBudgetPerDay = 80, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800", Description = "Land of the Pharaohs" },
            new Country { Id = 2, Name = "Japan", NameAr = "اليابان", NameEs = "Japón", NameFr = "Japon", NameZh = "日本", NameRu = "Япония", NameTr = "Japonya", Code = "JP", Continent = "Asia", AverageBudgetPerDay = 150, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800", Description = "Land of the Rising Sun" },
            new Country { Id = 3, Name = "France", NameAr = "فرنسا", NameEs = "Francia", NameFr = "France", NameZh = "法国", NameRu = "Франция", NameTr = "Fransa", Code = "FR", Continent = "Europe", AverageBudgetPerDay = 180, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1499856871958-5b9357976b82?w=800", Description = "City of Light and Art" },
            new Country { Id = 4, Name = "Thailand", NameAr = "تايلاند", NameEs = "Tailandia", NameFr = "Thaïlande", NameZh = "泰国", NameRu = "Таиланд", NameTr = "Tayland", Code = "TH", Continent = "Asia", AverageBudgetPerDay = 70, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1552465011-b4e21bf6e79a?w=800", Description = "Land of Smiles" },
            new Country { Id = 5, Name = "Italy", NameAr = "إيطاليا", NameEs = "Italia", NameFr = "Italie", NameZh = "意大利", NameRu = "Италия", NameTr = "İtalya", Code = "IT", Continent = "Europe", AverageBudgetPerDay = 160, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1523906834658-6e24ef2386f9?w=800", Description = "Eternal City & Cuisine" },
            new Country { Id = 6, Name = "USA", NameAr = "الولايات المتحدة", NameEs = "EE.UU.", NameFr = "États-Unis", NameZh = "美国", NameRu = "США", NameTr = "ABD", Code = "US", Continent = "Americas", AverageBudgetPerDay = 200, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1485738422979-f5c462d49f74?w=800", Description = "Land of Opportunities" },
            new Country { Id = 7, Name = "Maldives", NameAr = "جزر المالديف", NameEs = "Maldivas", NameFr = "Maldives", NameZh = "马尔代夫", NameRu = "Мальдивы", NameTr = "Maldivler", Code = "MV", Continent = "Asia", AverageBudgetPerDay = 350, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=800", Description = "Tropical Paradise" },
            new Country { Id = 8, Name = "Turkey", NameAr = "تركيا", NameEs = "Turquía", NameFr = "Turquie", NameZh = "土耳其", NameRu = "Турция", NameTr = "Türkiye", Code = "TR", Continent = "Europe", AverageBudgetPerDay = 90, IsPopular = true, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=800", Description = "Where East Meets West" }
        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Name = "Nile Ritz-Carlton Cairo", Description = "Iconic 5-star hotel overlooking the Nile River", City = "Cairo", PricePerNight = 320, Stars = 5, Rating = 4.8, ReviewCount = 2341, HasPool = true, HasWifi = true, HasSpa = true, IsLuxury = true, IsFeatured = true, CountryId = 1, ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800" },
            new Hotel { Id = 2, Name = "Park Hyatt Tokyo", Description = "Luxury high-rise with panoramic city views", City = "Tokyo", PricePerNight = 580, Stars = 5, Rating = 4.9, ReviewCount = 1876, HasPool = true, HasWifi = true, HasSpa = true, IsLuxury = true, IsFeatured = true, CountryId = 2, ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800" },
            new Hotel { Id = 3, Name = "Hôtel Le Meurice Paris", Description = "Palace hotel in the heart of Paris since 1835", City = "Paris", PricePerNight = 750, Stars = 5, Rating = 4.9, ReviewCount = 3201, HasPool = false, HasWifi = true, HasSpa = true, IsLuxury = true, IsFeatured = true, CountryId = 3, ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800" },
            new Hotel { Id = 4, Name = "Anantara Riverside Bangkok", Description = "Serene riverside retreat with Thai architecture", City = "Bangkok", PricePerNight = 210, Stars = 5, Rating = 4.7, ReviewCount = 1654, HasPool = true, HasWifi = true, HasSpa = true, IsLuxury = true, IsFeatured = true, CountryId = 4, ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800" },
            new Hotel { Id = 5, Name = "Four Seasons Florence", Description = "Renaissance palace turned luxury hotel", City = "Florence", PricePerNight = 680, Stars = 5, Rating = 4.9, ReviewCount = 2109, HasPool = true, HasWifi = true, HasSpa = true, IsLuxury = true, IsFeatured = true, CountryId = 5, ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=800" },
            new Hotel { Id = 6, Name = "Soneva Jani Maldives", Description = "Overwater villas in pristine turquoise lagoon", City = "Noonu Atoll", PricePerNight = 1800, Stars = 5, Rating = 5.0, ReviewCount = 987, HasPool = true, HasWifi = true, HasSpa = true, IsLuxury = true, IsFeatured = true, CountryId = 7, ImageUrl = "https://images.unsplash.com/photo-1439066290691-daf3a565efed?w=800" }
        );

        modelBuilder.Entity<Flight>().HasData(
            new Flight { Id = 1, Airline = "Emirates", AirlineCode = "EK", Origin = "Dubai", OriginCode = "DXB", Destination = "Cairo", DestinationCode = "CAI", DepartureTime = new DateTime(2026, 7, 15, 8, 0, 0, DateTimeKind.Utc), ArrivalTime = new DateTime(2026, 7, 15, 10, 30, 0, DateTimeKind.Utc), DurationMinutes = 150, Price = 320, Class = "Economy", IsDirectFlight = true, SeatsAvailable = 42, IsFeatured = true },
            new Flight { Id = 2, Airline = "Qatar Airways", AirlineCode = "QR", Origin = "Doha", OriginCode = "DOH", Destination = "Tokyo", DestinationCode = "NRT", DepartureTime = new DateTime(2026, 7, 20, 14, 0, 0, DateTimeKind.Utc), ArrivalTime = new DateTime(2026, 7, 21, 7, 0, 0, DateTimeKind.Utc), DurationMinutes = 720, Price = 890, Class = "Business", IsDirectFlight = false, SeatsAvailable = 8, IsFeatured = true },
            new Flight { Id = 3, Airline = "Air France", AirlineCode = "AF", Origin = "Paris", OriginCode = "CDG", Destination = "New York", DestinationCode = "JFK", DepartureTime = new DateTime(2026, 7, 18, 10, 30, 0, DateTimeKind.Utc), ArrivalTime = new DateTime(2026, 7, 18, 13, 0, 0, DateTimeKind.Utc), DurationMinutes = 510, Price = 650, Class = "Economy", IsDirectFlight = true, SeatsAvailable = 27, IsFeatured = true },
            new Flight { Id = 4, Airline = "Turkish Airlines", AirlineCode = "TK", Origin = "Istanbul", OriginCode = "IST", Destination = "Bangkok", DestinationCode = "BKK", DepartureTime = new DateTime(2026, 7, 22, 1, 0, 0, DateTimeKind.Utc), ArrivalTime = new DateTime(2026, 7, 22, 16, 0, 0, DateTimeKind.Utc), DurationMinutes = 600, Price = 540, Class = "Economy", IsDirectFlight = false, SeatsAvailable = 33, IsFeatured = true }
        );

        modelBuilder.Entity<Activity>().HasData(
            new Activity { Id = 1, Name = "Pyramids of Giza Tour", Description = "Explore the ancient wonders with expert guide", Category = "Adventure", City = "Cairo", Price = 85, DurationHours = 6, Rating = 4.9, ReviewCount = 5432, IsFeatured = true, IsPopular = true, CountryId = 1, ImageUrl = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800" },
            new Activity { Id = 2, Name = "Mount Fuji Hiking", Description = "Guided trek to Japan's iconic peak", Category = "Adventure", City = "Fujiyoshida", Price = 120, DurationHours = 10, Rating = 4.8, ReviewCount = 3211, IsFeatured = true, IsPopular = true, CountryId = 2, ImageUrl = "https://images.unsplash.com/photo-1490806843957-31f4c9a91c65?w=800" },
            new Activity { Id = 3, Name = "Eiffel Tower & Louvre", Description = "Paris highlights with skip-the-line access", Category = "Museums", City = "Paris", Price = 95, DurationHours = 8, Rating = 4.9, ReviewCount = 8765, IsFeatured = true, IsPopular = true, CountryId = 3, ImageUrl = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=800" },
            new Activity { Id = 4, Name = "Thai Cooking Masterclass", Description = "Learn authentic Thai cuisine from a local chef", Category = "Food", City = "Bangkok", Price = 55, DurationHours = 4, Rating = 4.9, ReviewCount = 2134, IsFeatured = true, IsPopular = true, CountryId = 4, ImageUrl = "https://images.unsplash.com/photo-1559314809-0d155014e29e?w=800" },
            new Activity { Id = 5, Name = "Venice Gondola & Rialto", Description = "Classic gondola ride through Venice canals", Category = "Cultural", City = "Venice", Price = 75, DurationHours = 3, Rating = 4.7, ReviewCount = 4321, IsFeatured = true, IsPopular = true, CountryId = 5, ImageUrl = "https://images.unsplash.com/photo-1534113414509-0eec2bfb493f?w=800" },
            new Activity { Id = 6, Name = "Maldives Snorkeling Safari", Description = "Discover vibrant coral reefs and marine life", Category = "Adventure", City = "Male", Price = 180, DurationHours = 5, Rating = 5.0, ReviewCount = 1876, IsFeatured = true, IsPopular = true, CountryId = 7, ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=800" }
        );
    }
}
