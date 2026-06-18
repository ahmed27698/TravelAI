using System.ComponentModel.DataAnnotations;

namespace TravelAI.Models;

public class Hotel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public double Stars { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool HasPool { get; set; }
    public bool HasWifi { get; set; }
    public bool HasSpa { get; set; }
    public bool IsLuxury { get; set; }
    public bool IsFeatured { get; set; }

    public int CountryId { get; set; }
    public Country? Country { get; set; }
}
