using System.ComponentModel.DataAnnotations;
public class CreatePatientRequest
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(400, MinimumLength = 1, ErrorMessage = "FullName must be between 1 and 400 characters")]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "DateOfBirth is required")]
    public required DateTime DateOfBirth { set; get; }

    [Required(ErrorMessage = "Gender is required")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Gender must be between 1 and 10 characters")]
    public required string Gender { get; set; }

    [Required(ErrorMessage = "PhoneNumber is required")]
    [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Invalid phone number")]
    [StringLength(11, ErrorMessage = "PhoneNumber must be 11 digits")]
    public required string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }
}