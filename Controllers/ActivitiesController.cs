using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;

namespace TravelAI.Controllers;

public class ActivitiesController : Controller
{
    private readonly TravelAIDbContext _db;

    public ActivitiesController(TravelAIDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? category, int? countryId)
    {
        var query = _db.Activities.Include(a => a.Country).AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category == category);
        if (countryId.HasValue)
            query = query.Where(a => a.CountryId == countryId.Value);

        ViewBag.Categories = await _db.Activities.Select(a => a.Category).Distinct().ToListAsync();
        ViewBag.Countries = await _db.Countries.ToListAsync();
        return View(await query.OrderByDescending(a => a.IsPopular).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var activity = await _db.Activities.Include(a => a.Country).FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null) return NotFound();
        return View(activity);
    }
}
