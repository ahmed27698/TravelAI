using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;
using TravelAI.Models.ViewModels;
using TravelAI.Services;

namespace TravelAI.Controllers;

public class HomeController : Controller
{
    private readonly TravelAIDbContext _db;
    private readonly EmailService _email;

    public HomeController(TravelAIDbContext db, EmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            FeaturedCountries = await _db.Countries.Where(c => c.IsPopular).Take(8).ToListAsync(),
            FeaturedHotels = await _db.Hotels.Include(h => h.Country).Where(h => h.IsFeatured).Take(6).ToListAsync(),
            FeaturedFlights = await _db.Flights.Where(f => f.IsFeatured).Take(4).ToListAsync(),
            FeaturedActivities = await _db.Activities.Include(a => a.Country).Where(a => a.IsFeatured).Take(6).ToListAsync()
        };
        return View(vm);
    }

    public IActionResult Privacy() => View();
    public IActionResult Terms() => View();
    public IActionResult Blog() => View();
    public IActionResult Faq() => View();

    [HttpGet]
    public IActionResult Contact() => View();

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> SendContact([FromBody] ContactRequest req)
    {
        if (string.IsNullOrWhiteSpace(req?.Name) || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Message))
            return Json(new { success = false, message = "Please fill in all required fields." });

        try
        {
            await _email.SendContactEmailAsync(req.Name.Trim(), req.Email.Trim(), req.Subject?.Trim() ?? "General Inquiry", req.Message.Trim());
            return Json(new { success = true, message = "Your message has been sent! We'll get back to you within 24 hours." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Failed to send message: {ex.Message}" });
        }
    }

    public new IActionResult StatusCode(int code)
    {
        if (code == 404) return View("~/Views/Shared/NotFound.cshtml");
        return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}

public record ContactRequest(string? Name, string? Email, string? Subject, string? Message);
