using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Dtos.Account;

public class AuthRequestDto
{
    [Required] public string? Username { get; set; }
    [Required] public string? Password { get; set; }
}