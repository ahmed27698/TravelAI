using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;

namespace TravelAI.Controllers;

public class HotelsController : Controller
{
    private readonly TravelAIDbContext _db;

    public HotelsController(TravelAIDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, int? countryId, decimal? maxPrice)
    {
        var query = _db.Hotels.Include(h => h.Country).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(h => h.Name.Contains(search) || h.City.Contains(search));
        if (countryId.HasValue)
            query = query.Where(h => h.CountryId == countryId.Value);
        if (maxPrice.HasValue)
            query = query.Where(h => h.PricePerNight <= maxPrice.Value);

        ViewBag.Countries = await _db.Countries.ToListAsync();
        return View(await query.OrderByDescending(h => h.IsFeatured).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var hotel = await _db.Hotels.Include(h => h.Country).FirstOrDefaultAsync(h => h.Id == id);
        if (hotel == null) return NotFound();
        return View(hotel);
    }
}
