using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Helpers;

public class ListsSearchQueryObject
{
    [Required(ErrorMessage = "Query parameter is required.")]
    [MinLength(1, ErrorMessage = "Query parameter cannot be empty.")]
    public string Query { get; set; } = string.Empty;
}