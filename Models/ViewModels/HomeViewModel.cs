namespace TravelAI.Models.ViewModels;

public class HomeViewModel
{
    public List<Country> FeaturedCountries { get; set; } = new();
    public List<Hotel> FeaturedHotels { get; set; } = new();
    public List<Flight> FeaturedFlights { get; set; } = new();
    public List<Activity> FeaturedActivities { get; set; } = new();
}
