namespace TravelAI.Models;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }

    public string Plan { get; set; } = "Free";
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string BillingCycle { get; set; } = "Monthly";
}
