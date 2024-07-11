using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestioApi.Dtos.CuratedList
{
    public class CuratedListWithUserDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string OwnerId { get; set; }
        // public bool IsPublic { get; set; } = false;
        public string ListType { get; set; } = string.Empty;
        public string? CoverImgUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //Owner props
        public string OwnerUsername { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerProfileImgUrl { get; set; }
    }
}