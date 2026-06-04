using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;
using TinyUrlApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();


static string GenerateShortCode()
{
    const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
}


app.MapPost("/api/shorten", async (ShortenRequest request, AppDbContext db, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(request.OriginalUrl) || !Uri.IsWellFormedUriString(request.OriginalUrl, UriKind.Absolute))
        return Results.BadRequest(new { error = "Invalid URL provided." });

    string shortCode;
    do { shortCode = GenerateShortCode(); }
    while (await db.UrlEntries.AnyAsync(u => u.ShortCode == shortCode));

    var entry = new UrlEntry
    {
        ShortCode = shortCode,
        OriginalUrl = request.OriginalUrl,
        CreatedAt = DateTime.UtcNow,
        IsPrivate = request.IsPrivate
    };

    db.UrlEntries.Add(entry);
    await db.SaveChangesAsync();

    var baseUrl = $"{http.Request.Scheme}://{http.Request.Host}";
    return Results.Created($"/api/urls/{shortCode}", new
    {
        entry.Id,
        entry.ShortCode,
        entry.OriginalUrl,
        ShortUrl = $"{baseUrl}/{shortCode}",
        entry.CreatedAt,
        entry.HitCount,
        entry.IsPrivate
    });
})
.WithName("ShortenUrl")
.WithTags("URLs")
.WithOpenApi();


app.MapGet("/api/urls", async (AppDbContext db, HttpContext http) =>
{
    var baseUrl = $"{http.Request.Scheme}://{http.Request.Host}";
    var entries = await db.UrlEntries
        .Where(u => !u.IsPrivate)
        .OrderByDescending(u => u.CreatedAt)
        .ToListAsync();
    return Results.Ok(entries.Select(e => new
    {
        e.Id,
        e.ShortCode,
        e.OriginalUrl,
        ShortUrl = $"{baseUrl}/{e.ShortCode}",
        e.CreatedAt,
        e.HitCount,
        e.IsPrivate
    }));
})
.WithName("GetAllUrls")
.WithTags("URLs")
.WithOpenApi();


app.MapGet("/api/urls/{shortCode}", async (string shortCode, AppDbContext db, HttpContext http) =>
{
    var entry = await db.UrlEntries.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    if (entry is null) return Results.NotFound(new { error = "Short code not found." });

    var baseUrl = $"{http.Request.Scheme}://{http.Request.Host}";
    return Results.Ok(new
    {
        entry.Id,
        entry.ShortCode,
        entry.OriginalUrl,
        ShortUrl = $"{baseUrl}/{entry.ShortCode}",
        entry.CreatedAt,
        entry.HitCount,
        entry.IsPrivate
    });
})
.WithName("GetUrlByCode")
.WithTags("URLs")
.WithOpenApi();


app.MapDelete("/api/urls/{shortCode}", async (string shortCode, AppDbContext db) =>
{
    var entry = await db.UrlEntries.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    if (entry is null) return Results.NotFound(new { error = "Short code not found." });

    db.UrlEntries.Remove(entry);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = $"Short code '{shortCode}' deleted." });
})
.WithName("DeleteUrl")
.WithTags("URLs")
.WithOpenApi();


app.MapGet("/{shortCode}", async (string shortCode, AppDbContext db) =>
{
    var entry = await db.UrlEntries.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    if (entry is null) return Results.NotFound(new { error = "Short code not found." });

    entry.HitCount++;
    await db.SaveChangesAsync();

    return Results.Redirect(entry.OriginalUrl, permanent: false);
})
.WithName("RedirectToUrl")
.WithTags("Redirect")
.WithOpenApi();

app.Run();


record ShortenRequest(string OriginalUrl, bool IsPrivate = false);
