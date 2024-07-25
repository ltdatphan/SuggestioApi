using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace SuggestioApi.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImgUrl { get; set; }

    [JsonIgnore] public List<RefreshToken> RefreshTokens { get; set; } = [];

    // Calculated Property
    [NotMapped] public int FollowersCount => Followers?.Count() ?? 0;

    [NotMapped] public int FollowingsCount => Following?.Count() ?? 0;

    //Navigation Property
    public ICollection<CuratedList> CuratedLists { get; set; } = [];
    public ICollection<Follow> Followers { get; set; } = []; // List of people who followed this person
    public ICollection<Follow> Following { get; set; } = []; // List of users this person followed
}