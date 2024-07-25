using SuggestioApi.Dtos.Item;

namespace SuggestioApi.Dtos.CuratedList;

public class CuratedListWithItemsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;
    public string ListType { get; set; } = string.Empty;
    public string? CoverImgUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<ItemDto> ListItems { get; set; } = new();
}