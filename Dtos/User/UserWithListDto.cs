using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.CuratedList;


namespace SuggestioApi.Dtos.User
{
    public class UserWithListDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImgUrl { get; set; }

        // Derived prop. Depending on the user viewing this users profile.
        public bool IsFollowing { get; set; } = false;

        //Users lists
        public List<BasicCuratedListDto> UserLists { get; set; } = new List<BasicCuratedListDto>();
    }
}