namespace TravelAI.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var h = context.Response.Headers;

        h["X-Frame-Options"]           = "DENY";
        h["X-Content-Type-Options"]    = "nosniff";
        h["X-XSS-Protection"]          = "1; mode=block";
        h["Referrer-Policy"]           = "strict-origin-when-cross-origin";
        h["Permissions-Policy"]        = "geolocation=(), microphone=(), camera=(), payment=(), usb=()";
        h["X-Permitted-Cross-Domain-Policies"] = "none";
        h["Content-Security-Policy"]   =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' https://cdn.tailwindcss.com https://cdnjs.cloudflare.com; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdnjs.cloudflare.com; " +
            "font-src 'self' data: https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
            "img-src 'self' data: blob: https://images.unsplash.com https://flagcdn.com https://cdnjs.cloudflare.com; " +
            "connect-src 'self'; " +
            "frame-src 'none'; " +
            "frame-ancestors 'none'; " +
            "object-src 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self';";

        await next(context);
    }
}

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}
