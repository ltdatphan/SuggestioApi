namespace SuggestioApi.Dtos.CuratedList;

public class CuratedListWithUserDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }

    public string OwnerId { get; set; } = string.Empty;

    // public bool IsPublic { get; set; } = false;
    public string ListType { get; set; } = string.Empty;
    public string? CoverImgUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //Owner props
    public string OwnerUsername { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string OwnerProfileImgUrl { get; set; } = string.Empty;
}