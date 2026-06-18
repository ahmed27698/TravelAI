using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelAI.Controllers;

[Authorize]
public class TripPlannerController : Controller
{
    public IActionResult Index() => View();
}
