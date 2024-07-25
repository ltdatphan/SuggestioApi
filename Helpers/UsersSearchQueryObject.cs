using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Helpers;

public class UsersSearchQueryObject
{
    [Required(ErrorMessage = "Query parameter is required.")]
    [MinLength(1, ErrorMessage = "Query parameter cannot be empty.")]
    public string Query { get; set; } = string.Empty;

    // public UserQueryObject UserQueryObject { get; set; } = new UserQueryObject();
}