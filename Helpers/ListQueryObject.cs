using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Helpers;

public class ListQueryObject
{
    [QueryObjectValidation.AllowedValues(["public", "private"])] public string? Access { get; set; } = null;

    [QueryObjectValidation.AllowedValues(["Title", "CreatedAt", "UpdatedAt", "ItemCount"])]
    public string? SortBy { get; set; } = null; //Key used to sort 

    public bool IsDescending { get; set; } = false;

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 20, ErrorMessage = "Page size must be between 1 and 20.")]
    public int PageSize { get; set; } = 20; //20 items per page
}