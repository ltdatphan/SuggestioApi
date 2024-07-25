using System.ComponentModel.DataAnnotations;

namespace SuggestioApi.Dtos.Account;

public class RegisterDto
{
    [Required] public string? Username { get; set; }

    [Required] [EmailAddress] public string? Email { get; set; }

    [Required] public string? Password { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string? LastName { get; set; }
}