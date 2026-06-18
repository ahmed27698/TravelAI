using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelAI.Controllers;

public class SubscriptionController : Controller
{
    [Authorize]
    public IActionResult Index() => View();

    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Upgrade(string plan)
    {
        TempData["Message"] = $"Thank you! Your upgrade to {plan} is being processed.";
        return RedirectToAction("Index");
    }
}
