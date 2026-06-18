using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;

namespace TravelAI.Controllers;

public class FlightsController : Controller
{
    private readonly TravelAIDbContext _db;

    public FlightsController(TravelAIDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? origin, string? destination, string? flightClass)
    {
        var query = _db.Flights.AsQueryable();

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(f => f.Origin.Contains(origin) || f.OriginCode.Contains(origin));
        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(f => f.Destination.Contains(destination) || f.DestinationCode.Contains(destination));
        if (!string.IsNullOrWhiteSpace(flightClass))
            query = query.Where(f => f.Class == flightClass);

        return View(await query.OrderBy(f => f.Price).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var flight = await _db.Flights.FindAsync(id);
        if (flight == null) return NotFound();
        return View(flight);
    }
}
