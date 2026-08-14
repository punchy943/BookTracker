using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "E-mail adres is verplicht.")]
    [EmailAddress(ErrorMessage = "Voer een geldig E-mail adres in.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wachtwoord is verplicht.")]
    public string Password { get; set; } = string.Empty;
}