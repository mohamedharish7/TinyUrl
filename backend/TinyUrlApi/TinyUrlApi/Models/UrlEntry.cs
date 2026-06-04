namespace TinyUrlApi.Models;

public class UrlEntry
{
    public int Id { get; set; }
    public string ShortCode { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int HitCount { get; set; } = 0;
    public bool IsPrivate { get; set; } = false;
}
