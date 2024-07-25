namespace SuggestioApi.Dtos.CuratedList;

//Similar to CuratedListDto but with less info
public class BasicCuratedListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;
    public string? CoverImgUrl { get; set; }
    public int ItemCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}