using System.ComponentModel.DataAnnotations;

namespace TravelAI.Models;

public class Flight
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Airline { get; set; } = string.Empty;

    public string AirlineCode { get; set; } = string.Empty;
    public string AirlineLogo { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string OriginCode { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string DestinationCode { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string Class { get; set; } = "Economy";
    public bool IsDirectFlight { get; set; }
    public int SeatsAvailable { get; set; }
    public bool IsFeatured { get; set; }
}
