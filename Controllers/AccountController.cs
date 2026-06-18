using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;
using TravelAI.Models.ViewModels;

namespace TravelAI.Controllers;

public class AccountController : Controller
{
    private readonly TravelAIDbContext _db;

    public AccountController(TravelAIDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email.Trim().ToLower());

        // Account lockout check
        if (user?.LockoutEnd.HasValue == true && user.LockoutEnd > DateTime.UtcNow)
        {
            var mins = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            ModelState.AddModelError(string.Empty, $"Account locked due to too many failed attempts. Try again in {mins} minute(s).");
            return View(model);
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            if (user != null)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;
                    ModelState.AddModelError(string.Empty, "Account locked for 15 minutes due to too many failed attempts.");
                }
                else
                {
                    var left = 5 - user.FailedLoginAttempts;
                    ModelState.AddModelError(string.Empty, $"Invalid email or password. {left} attempt(s) remaining before lockout.");
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(model);
        }

        // Successful login — reset lockout counters
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new("IsPremium", user.IsPremium.ToString()),
            new("IsAdmin", user.IsAdmin.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var props = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), props);

        return !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? Redirect(model.ReturnUrl)
            : RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var normalizedEmail = model.Email.Trim().ToLower();

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
        {
            ModelState.AddModelError("Email", "An account with this email already exists.");
            return View(model);
        }

        var user = new AppUser
        {
            FullName = model.FullName,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var sub = new Subscription { UserId = user.Id, Plan = "Free", Price = 0 };
        _db.Subscriptions.Add(sub);
        await _db.SaveChangesAsync();

        return RedirectToAction("Login", new { message = "registered" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return RedirectToAction("Logout");
        return View(user);
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string fullName)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(fullName) && fullName.Length <= 100)
        {
            user.FullName = fullName.Trim();
            await _db.SaveChangesAsync();
            TempData["ProfileSuccess"] = "Profile updated successfully.";
        }
        return RedirectToAction("Profile");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ViewBag.Error = "Please enter your email address.";
            return View();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
        if (user != null)
        {
            user.ResetToken = Guid.NewGuid().ToString("N");
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _db.SaveChangesAsync();
            TempData["ResetToken"] = user.ResetToken;
        }

        TempData["ResetEmail"] = email.Trim();
        return RedirectToAction("ForgotPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation() => View();

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login");
        var user = await _db.Users.FirstOrDefaultAsync(u => u.ResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null)
        {
            TempData["ResetError"] = "This password reset link has expired or is invalid.";
            return RedirectToAction("ForgotPassword");
        }
        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null)
        {
            TempData["ResetError"] = "This password reset link has expired or is invalid.";
            return RedirectToAction("ForgotPassword");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        await _db.SaveChangesAsync();

        TempData["Message"] = "password_reset";
        return RedirectToAction("Login");
    }
}
