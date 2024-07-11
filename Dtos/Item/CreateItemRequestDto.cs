using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestioApi.Dtos.Item
{
    public class CreateItemRequestDto
    {
        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(150, ErrorMessage = "Item name cannot exceed 150 characters.")]
        public string ItemName { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Subtitle cannot exceed 200 characters.")]
        public string? Subtitle { get; set; }
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string? Category { get; set; }
        [Url(ErrorMessage = "Invalid URL format for Item Image Url.")]
        public string? ItemImgUrl { get; set; }
        [Url(ErrorMessage = "Invalid URL format for Item Url.")]
        public string? ItemUrl { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1-5.")]
        public float? Rating { get; set; }
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}