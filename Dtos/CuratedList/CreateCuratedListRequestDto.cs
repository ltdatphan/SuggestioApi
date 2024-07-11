using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestioApi.Dtos.CuratedList
{
    public class CreateCuratedListRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Subtitle cannot exceed 200 characters.")]
        public string? Subtitle { get; set; }
        [Required(ErrorMessage = "IsPublic is required.")]
        public bool IsPublic { get; set; } = false;
        [Required(ErrorMessage = "ListType is required.")]
        [StringLength(100, ErrorMessage = "ListType cannot exceed 100 characters.")]
        public string ListType { get; set; } = string.Empty;
        [Url(ErrorMessage = "Invalid URL for CoverImgUrl.")]
        public string? CoverImgUrl { get; set; }
    }
}