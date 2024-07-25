namespace SuggestioApi.Dtos.CuratedList;

//Similar to CuratedListDto but with less info
public class BasicCuratedListPublicDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImgUrl { get; set; }
    public string OwnerUsername { get; set; } = string.Empty;
    public string? OwnerProfileImgUrl { get; set; } = string.Empty;
    public int ItemCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}