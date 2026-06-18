using System.ComponentModel.DataAnnotations;

namespace TravelAI.Models;

public class Country
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameEs { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameFr { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameZh { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameRu { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameTr { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Continent { get; set; } = string.Empty;
    public decimal AverageBudgetPerDay { get; set; }
    public bool IsPopular { get; set; }
    public int Rating { get; set; } = 5;

    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
