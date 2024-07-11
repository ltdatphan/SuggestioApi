using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Models;

public class CuratedList
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string OwnerId { get; set; }
    public bool IsPublic { get; set; } = false;
    public string ListType { get; set; } = string.Empty;
    public string? CoverImgUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Property
    public User User { get; set; }
    public ICollection<Item> Items { get; set; } = [];
}