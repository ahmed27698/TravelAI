using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Middleware;
using TravelAI.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Remove Server header ──────────────────────────────────────────────────
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<TravelAIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Authentication / Cookie hardening ────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath         = "/Account/Login";
        options.LogoutPath        = "/Account/Logout";
        options.AccessDeniedPath  = "/Account/Login";
        options.ExpireTimeSpan    = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly   = true;
        options.Cookie.SameSite   = SameSiteMode.Strict;
        // __Host- prefix requires Secure flag; only use it in production (HTTPS).
        // Over plain HTTP the browser rejects the cookie silently, breaking login.
        if (builder.Environment.IsProduction())
        {
            options.Cookie.Name         = "__Host-TravelAI";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }
        else
        {
            options.Cookie.Name         = "TravelAI";
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        }
    });

// ── HSTS (production) ─────────────────────────────────────────────────────
builder.Services.AddHsts(o =>
{
    o.MaxAge           = TimeSpan.FromDays(365);
    o.IncludeSubDomains = true;
    o.Preload          = true;
});

// ── Rate Limiting ─────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.OnRejected = async (ctx, token) =>
    {
        ctx.HttpContext.Response.StatusCode = 429;
        var isHtml = ctx.HttpContext.Request.Headers.Accept
            .Any(a => a?.Contains("text/html") == true);
        if (isHtml)
        {
            ctx.HttpContext.Response.ContentType = "text/html; charset=utf-8";
            await ctx.HttpContext.Response.WriteAsync(
                "<!DOCTYPE html><html><head><meta charset='utf-8'/>" +
                "<meta http-equiv='refresh' content='5;url=javascript:history.back()'/></head>" +
                "<body style='font-family:sans-serif;background:#030712;color:#e2e8f0;display:flex;align-items:center;justify-content:center;min-height:100vh;margin:0;'>" +
                "<div style='text-align:center;'><h2 style='color:#f87171;'>Too Many Requests</h2>" +
                "<p style='color:#94a3b8;'>Please wait a moment and try again.</p>" +
                "<a href='javascript:history.back()' style='color:#60a5fa;'>Go Back</a></div></body></html>",
                cancellationToken: token);
        }
        else
        {
            ctx.HttpContext.Response.ContentType = "application/json";
            await ctx.HttpContext.Response.WriteAsync(
                "{\"error\":\"Too many requests. Please try again later.\"}",
                cancellationToken: token);
        }
    };

    // Login: 5 attempts per 15 minutes per IP
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window      = TimeSpan.FromMinutes(15),
                QueueLimit  = 0,
            }));

    // Register / ForgotPassword: 3 per hour per IP
    options.AddPolicy("register", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window      = TimeSpan.FromHours(1),
                QueueLimit  = 0,
            }));

    // Contact form: 5 per hour per IP
    options.AddPolicy("contact", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window      = TimeSpan.FromHours(1),
                QueueLimit  = 0,
            }));

    // Chat API: 30 per minute per IP
    options.AddPolicy("chat", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit       = 30,
                Window            = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,
                QueueLimit        = 0,
            }));
});

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddSingleton<EmailService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddSignalR();
builder.Services.AddAntiforgery(o =>
{
    o.Cookie.HttpOnly  = true;
    o.Cookie.SameSite  = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    o.HeaderName       = "X-CSRF-TOKEN";
});

var app = builder.Build();

// ── Apply pending migrations ───────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TravelAIDbContext>();
    db.Database.Migrate();
}

// ── Middleware pipeline ───────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSecurityHeaders();
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
