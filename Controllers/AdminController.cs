using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly TravelAIDbContext _db;
    public AdminController(TravelAIDbContext db) => _db = db;

    // ── Dashboard ────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsers      = await _db.Users.CountAsync();
        ViewBag.PremiumUsers    = await _db.Users.CountAsync(u => u.IsPremium);
        ViewBag.NewUsersMonth   = await _db.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));
        ViewBag.LockedUsers     = await _db.Users.CountAsync(u => u.LockoutEnd != null && u.LockoutEnd > DateTime.UtcNow);
        ViewBag.TotalCountries  = await _db.Countries.CountAsync();
        ViewBag.TotalHotels     = await _db.Hotels.CountAsync();
        ViewBag.TotalFlights    = await _db.Flights.CountAsync();
        ViewBag.TotalActivities = await _db.Activities.CountAsync();
        ViewBag.ActiveSubs      = await _db.Subscriptions.CountAsync(s => s.IsActive && s.Plan != "Free");
        ViewBag.MonthlyRevenue  = await _db.Subscriptions
            .Where(s => s.IsActive && s.Plan != "Free")
            .SumAsync(s => (decimal?)s.Price) ?? 0m;
        ViewBag.RecentUsers = await _db.Users
            .OrderByDescending(u => u.CreatedAt).Take(6).ToListAsync();
        ViewBag.RecentSubs = await _db.Subscriptions
            .Include(s => s.User)
            .Where(s => s.Plan != "Free")
            .OrderByDescending(s => s.StartDate).Take(5).ToListAsync();
        return View();
    }

    // ── Users ────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Users(string? search)
    {
        var q = _db.Users.Include(u => u.Subscription).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        ViewBag.Search = search;
        return View(await q.OrderByDescending(u => u.CreatedAt).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePremium(int id, string? search)
    {
        var u = await _db.Users.FindAsync(id);
        if (u != null) { u.IsPremium = !u.IsPremium; await _db.SaveChangesAsync(); }
        return RedirectToAction("Users", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(int id, string? search)
    {
        var me = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id != me)
        {
            var u = await _db.Users.FindAsync(id);
            if (u != null) { u.IsAdmin = !u.IsAdmin; await _db.SaveChangesAsync(); }
        }
        return RedirectToAction("Users", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id, string? search)
    {
        var me = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id != me)
        {
            var u = await _db.Users.FindAsync(id);
            if (u != null) { _db.Users.Remove(u); await _db.SaveChangesAsync(); }
        }
        return RedirectToAction("Users", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlockUser(int id, string? search)
    {
        var u = await _db.Users.FindAsync(id);
        if (u != null) { u.LockoutEnd = null; u.FailedLoginAttempts = 0; await _db.SaveChangesAsync(); }
        return RedirectToAction("Users", new { search });
    }

    // ── Create Hotel ─────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHotel(Hotel hotel)
    {
        _db.Hotels.Add(hotel);
        await _db.SaveChangesAsync();
        return RedirectToAction("Hotels");
    }

    // ── Create Country ───────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCountry(Country country)
    {
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        return RedirectToAction("Countries");
    }

    // ── Create Flight ─────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFlight(Flight flight)
    {
        _db.Flights.Add(flight);
        await _db.SaveChangesAsync();
        return RedirectToAction("Flights");
    }

    // ── Create Activity ───────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateActivity(Activity activity)
    {
        _db.Activities.Add(activity);
        await _db.SaveChangesAsync();
        return RedirectToAction("Activities");
    }

    // ── Countries ────────────────────────────────────────────────────────────
    public async Task<IActionResult> Countries(string? search)
    {
        var q = _db.Countries
            .Include(c => c.Hotels)
            .Include(c => c.Activities)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
        ViewBag.Search = search;
        return View(await q.OrderBy(c => c.Name).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePopular(int id, string? search)
    {
        var c = await _db.Countries.FindAsync(id);
        if (c != null) { c.IsPopular = !c.IsPopular; await _db.SaveChangesAsync(); }
        return RedirectToAction("Countries", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCountry(int id, string? search)
    {
        var c = await _db.Countries
            .Include(x => x.Hotels)
            .Include(x => x.Activities)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (c != null && !c.Hotels.Any() && !c.Activities.Any())
        {
            _db.Countries.Remove(c);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Countries", new { search });
    }

    // ── Hotels ───────────────────────────────────────────────────────────────
    public async Task<IActionResult> Hotels(string? search, int? countryId)
    {
        var q = _db.Hotels.Include(h => h.Country).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(h => h.Name.Contains(search) || h.City.Contains(search));
        if (countryId.HasValue)
            q = q.Where(h => h.CountryId == countryId);
        ViewBag.Search    = search;
        ViewBag.CountryId = countryId;
        ViewBag.Countries = await _db.Countries.OrderBy(c => c.Name).ToListAsync();
        return View(await q.OrderByDescending(h => h.IsFeatured).ThenBy(h => h.Name).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleHotelFeatured(int id, string? search)
    {
        var h = await _db.Hotels.FindAsync(id);
        if (h != null) { h.IsFeatured = !h.IsFeatured; await _db.SaveChangesAsync(); }
        return RedirectToAction("Hotels", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteHotel(int id, string? search)
    {
        var h = await _db.Hotels.FindAsync(id);
        if (h != null) { _db.Hotels.Remove(h); await _db.SaveChangesAsync(); }
        return RedirectToAction("Hotels", new { search });
    }

    // ── Flights ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> Flights(string? search)
    {
        var q = _db.Flights.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(f => f.Airline.Contains(search) || f.Origin.Contains(search) || f.Destination.Contains(search));
        ViewBag.Search = search;
        return View(await q.OrderByDescending(f => f.IsFeatured).ThenBy(f => f.DepartureTime).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFlightFeatured(int id, string? search)
    {
        var f = await _db.Flights.FindAsync(id);
        if (f != null) { f.IsFeatured = !f.IsFeatured; await _db.SaveChangesAsync(); }
        return RedirectToAction("Flights", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFlight(int id, string? search)
    {
        var f = await _db.Flights.FindAsync(id);
        if (f != null) { _db.Flights.Remove(f); await _db.SaveChangesAsync(); }
        return RedirectToAction("Flights", new { search });
    }

    // ── Activities ───────────────────────────────────────────────────────────
    public async Task<IActionResult> Activities(string? search, int? countryId)
    {
        var q = _db.Activities.Include(a => a.Country).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(a => a.Name.Contains(search) || a.City.Contains(search));
        if (countryId.HasValue)
            q = q.Where(a => a.CountryId == countryId);
        ViewBag.Search    = search;
        ViewBag.CountryId = countryId;
        ViewBag.Countries = await _db.Countries.OrderBy(c => c.Name).ToListAsync();
        return View(await q.OrderByDescending(a => a.IsFeatured).ThenBy(a => a.Name).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivityFeatured(int id, string? search)
    {
        var a = await _db.Activities.FindAsync(id);
        if (a != null) { a.IsFeatured = !a.IsFeatured; await _db.SaveChangesAsync(); }
        return RedirectToAction("Activities", new { search });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteActivity(int id, string? search)
    {
        var a = await _db.Activities.FindAsync(id);
        if (a != null) { _db.Activities.Remove(a); await _db.SaveChangesAsync(); }
        return RedirectToAction("Activities", new { search });
    }

    // ── Subscriptions ────────────────────────────────────────────────────────
    public async Task<IActionResult> Subscriptions(string? plan)
    {
        var q = _db.Subscriptions.Include(s => s.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(plan))
            q = q.Where(s => s.Plan == plan);
        ViewBag.Plan      = plan;
        ViewBag.FreeCount = await _db.Subscriptions.CountAsync(s => s.Plan == "Free");
        ViewBag.PaidCount = await _db.Subscriptions.CountAsync(s => s.IsActive && s.Plan != "Free");
        ViewBag.Revenue   = await _db.Subscriptions
            .Where(s => s.IsActive && s.Plan != "Free")
            .SumAsync(s => (decimal?)s.Price) ?? 0m;
        return View(await q.OrderByDescending(s => s.StartDate).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSubActive(int id, string? plan)
    {
        var s = await _db.Subscriptions.FindAsync(id);
        if (s != null) { s.IsActive = !s.IsActive; await _db.SaveChangesAsync(); }
        return RedirectToAction("Subscriptions", new { plan });
    }
}
