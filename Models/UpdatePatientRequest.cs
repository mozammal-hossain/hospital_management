using System.ComponentModel.DataAnnotations;

public class UpdatePatientRequest
{

    [StringLength(400, MinimumLength = 1, ErrorMessage = "FullName must be between 1 and 400 characters")]
    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    [StringLength(10, MinimumLength = 1, ErrorMessage = "Gender must be between 1 and 10 characters")]
    public string? Gender { get; set; }

    [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Invalid phone number")]
    [StringLength(11, ErrorMessage = "PhoneNumber must be 11 digits")]
    public string? PhoneNumber { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }
    public DateTime? AdmittedAt { get; set; }
    public bool? IsDischarged { get; set; }
}