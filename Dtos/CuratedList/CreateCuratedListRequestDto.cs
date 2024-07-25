using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Dtos.CuratedList;

public class CreateCuratedListRequestDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Subtitle cannot exceed 100 characters.")]
    public string? Subtitle { get; set; }

    [Required(ErrorMessage = "IsPublic is required.")]
    public bool IsPublic { get; set; } = false;

    [Required(ErrorMessage = "ListType is required.")]
    [StringLength(50, ErrorMessage = "ListType cannot exceed 50 characters.")]
    public string ListType { get; set; } = string.Empty;

    [Url(ErrorMessage = "Invalid URL for CoverImgUrl.")]
    public string? CoverImgUrl { get; set; }
}