using System.ComponentModel.DataAnnotations;

namespace Noitcua.Models;
public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    [MaxLength(50)] // Assuming the maximum length for email is 50 characters as per bdContext
    public string Email { get; set; }

    [Required]
    [MaxLength(20)] // Assuming the maximum length for handle is 20 characters
    public string Handle { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)] // Assuming a minimum length of 8 characters for passwords
    [MaxLength(50)] // Assuming the maximum length for password is 50 characters
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}

