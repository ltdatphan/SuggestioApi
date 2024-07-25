namespace SuggestioApi.Models;

public class Item
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ListId { get; set; }
    public string? Subtitle { get; set; }
    public string? Category { get; set; }
    public string? ItemImgUrl { get; set; }
    public string? ItemUrl { get; set; }
    public float? Rating { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Property
    public CuratedList CuratedList { get; set; } = null!;
}