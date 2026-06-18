using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;

namespace TravelAI.Controllers;

public class CountriesController : Controller
{
    private readonly TravelAIDbContext _db;

    public CountriesController(TravelAIDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var countries = await _db.Countries.OrderByDescending(c => c.IsPopular).ToListAsync();
        return View(countries);
    }

    public async Task<IActionResult> Details(int id)
    {
        var country = await _db.Countries
            .Include(c => c.Hotels)
            .Include(c => c.Activities)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (country == null) return NotFound();
        return View(country);
    }
}
