using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Dtos.Item;

public class CreateItemRequestDto
{
    [Required(ErrorMessage = "Item name is required.")]
    [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters.")]
    public string ItemName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Subtitle cannot exceed 100 characters.")]
    public string? Subtitle { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
    public string? Category { get; set; }

    [Url(ErrorMessage = "Invalid URL format for Item Image Url.")]
    public string? ItemImgUrl { get; set; }

    [Url(ErrorMessage = "Invalid URL format for Item Url.")]
    public string? ItemUrl { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1-5.")]
    public float? Rating { get; set; }

    [StringLength(200, ErrorMessage = "Notes cannot exceed 200 characters.")]
    public string? Notes { get; set; }
}